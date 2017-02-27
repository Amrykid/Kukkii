using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.ExceptionServices;

namespace Kukkii.Containers
{
    public class PersistentCookieContainer : BasicCookieContainer, IPersistentCookieContainer
    {
        protected ICookieFileSystemProvider fileSystemProvider = null;
        protected volatile bool cacheLoaded = false;
        private bool containerBroken = false;
        protected string contextInfo = "persistent_cache";
        private JsonSerializer serializer = null;
        protected bool providerIsLocal = false;
        protected System.Threading.SemaphoreSlim initializeLock = null;
        internal PersistentCookieContainer(ICookieFileSystemProvider filesystem, bool isLocal)
            : base()
        {
            fileSystemProvider = filesystem;

            providerIsLocal = isLocal;

            initializeLock = new System.Threading.SemaphoreSlim(1);

            serializer = new JsonSerializer();
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            serializer.MaxDepth = 2048;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        }

        protected virtual async Task InitializeCacheIfNotDoneAlreadyAsync(ICookieFileSystemProvider filesystem)
        {
            if (containerBroken) throw new CacheCannotBeLoadedException("Container broken. It must be regenerated.");

            if (!cacheLoaded)
            {
                await CacheLock.WaitAsync();
                await initializeLock.WaitAsync();

                if (!cacheLoaded && !containerBroken) //after waiting for its turn, if the cache /still/ isn't loaded, try again.
                {
                    try
                    {
                        //load cache from disk

                        var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo);

                        if (data != null)
                        {
                            LoadCacheFromData(data);
                        }

                        cacheLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        containerBroken = true;

                        initializeLock.Release();
                        CacheLock.Release();

                        throw new CacheCannotBeLoadedException("Unable to load cache.", ex);
                    }
                }

                if (containerBroken) throw new CacheCannotBeLoadedException("Container broken. It must be regenerated.");

                initializeLock.Release();
                CacheLock.Release();
            }
        }

        protected void LoadCacheFromData(byte[] data)
        {
            try
            {
                using (StringReader sr = new StringReader(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length)))
                {
                    using (JsonTextReader jtr = new JsonTextReader(sr))
                    {
                        lock (Cache)
                        {
                            Cache = serializer.Deserialize<List<CookieDataPacket<object>>>(jtr);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                var exceptionInfo = ExceptionDispatchInfo.Capture(ex);
                exceptionInfo.Throw();
            }
        }

        public override async Task PushObjectAsync<T>(string key, T item, int expirationTime = -1)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.PushObjectAsync<T>(key, item, expirationTime);
        }

        public override async Task<bool> ContainsObjectAsync(string key)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.ContainsObjectAsync(key);
        }

        public override async System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.GetObjectAsync<T>(key, creationFunction);
        }

        public override async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.PeekObjectAsync<T>(key, creationFunction);
        }

        public override async System.Threading.Tasks.Task CleanUpAsync()
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.CleanUpAsync();
        }
        /// <summary>
        /// Saves the current cache to disk.
        /// </summary>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task FlushAsync()
        {
            await CacheLock.WaitAsync();

            //save cache to disk

            string json = null;
            using (StringWriter sw = new StringWriter())
            {
                using (JsonTextWriter jtw = new JsonTextWriter(sw))
                {
                    serializer.Serialize(jtw, Cache);
                }

                json = sw.ToString();
            }

            await WriteDataViaFileSystemAsync(System.Text.UTF8Encoding.UTF8.GetBytes(json));

            CacheLock.Release();
        }

        protected Task WriteDataViaFileSystemAsync(byte[] data)
        {
            return fileSystemProvider.SaveFileAsync(CookieJar.ApplicationName, contextInfo, data);
        }

        public async Task ReloadCacheAsync()
        {
            if (!cacheLoaded || fileSystemProvider == null) throw new InvalidOperationException();

            await CacheLock.WaitAsync();

            Cache = null;
            cacheLoaded = false;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            CacheLock.Release();

            if (CacheReloaded != null)
                CacheReloaded(this, new CookieCacheReloadedEventArgs());
        }

        public async Task RegenerateCacheAsync()
        {
            CacheLock.Dispose();
            CacheLock = new System.Threading.SemaphoreSlim(1);

            await CacheLock.WaitAsync();

            Cache.Clear();
            cacheLoaded = false;
            containerBroken = false;

            initializeLock.Dispose();
            initializeLock = new System.Threading.SemaphoreSlim(1);

            await fileSystemProvider.DeleteFileAsync(CookieJar.ApplicationName, contextInfo);

            CacheLock.Release();

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);
        }

        public bool IsCacheLoaded
        {
            get { return cacheLoaded; }
        }


        public async Task InitializeAsync()
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);
        }

        public event EventHandler<CookieCacheReloadedEventArgs> CacheReloaded;
    }
}
