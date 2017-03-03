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
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            serializer.MaxDepth = 4096;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
        }

        protected virtual async Task InitializeCacheIfNotDoneAlreadyAsync(ICookieFileSystemProvider filesystem)
        {
            if (containerBroken) throw new CacheCannotBeLoadedException("Container broken. It must be regenerated.");

            if (!cacheLoaded)
            {
                await CacheLock.WaitAsync().ConfigureAwait(false);
                await initializeLock.WaitAsync().ConfigureAwait(false);

                if (!cacheLoaded && !containerBroken) //after waiting for its turn, if the cache /still/ isn't loaded, try again.
                {
                    try
                    {
                        //load cache from disk

                        var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo).ConfigureAwait(false);

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
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            await base.PushObjectAsync<T>(key, item, expirationTime).ConfigureAwait(false);
        }

        public override async Task<bool> ContainsObjectAsync(string key)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            return await base.ContainsObjectAsync(key).ConfigureAwait(false);
        }

        public override async System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            return await base.GetObjectAsync<T>(key, creationFunction).ConfigureAwait(false);
        }

        public override async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            return await base.PeekObjectAsync<T>(key, creationFunction).ConfigureAwait(false);
        }

        public override async System.Threading.Tasks.Task CleanUpAsync()
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            await base.CleanUpAsync().ConfigureAwait(false);
        }
        /// <summary>
        /// Saves the current cache to disk.
        /// </summary>
        /// <returns></returns>
        public override async System.Threading.Tasks.Task FlushAsync()
        {
            await CacheLock.WaitAsync().ConfigureAwait(false);

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

            await WriteDataViaFileSystemAsync(System.Text.UTF8Encoding.UTF8.GetBytes(json)).ConfigureAwait(false);

            CacheLock.Release();
        }

        protected Task WriteDataViaFileSystemAsync(byte[] data)
        {
            return fileSystemProvider.SaveFileAsync(CookieJar.ApplicationName, contextInfo, data);
        }

        public async Task ReloadCacheAsync()
        {
            if (!cacheLoaded || fileSystemProvider == null) throw new InvalidOperationException();

            await CacheLock.WaitAsync().ConfigureAwait(false);

            Cache = null;
            cacheLoaded = false;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);

            CacheLock.Release();

            if (CacheReloaded != null)
                CacheReloaded(this, new CookieCacheReloadedEventArgs());
        }

        public async Task RegenerateCacheAsync()
        {
            CacheLock.Dispose();
            CacheLock = new System.Threading.SemaphoreSlim(1);

            await CacheLock.WaitAsync().ConfigureAwait(false);

            Cache.Clear();
            cacheLoaded = false;
            containerBroken = false;

            initializeLock.Dispose();
            initializeLock = new System.Threading.SemaphoreSlim(1);

            await fileSystemProvider.DeleteFileAsync(CookieJar.ApplicationName, contextInfo).ConfigureAwait(false);

            CacheLock.Release();

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);
        }

        public bool IsCacheLoaded
        {
            get { return cacheLoaded; }
        }


        public async Task InitializeAsync()
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider).ConfigureAwait(false);
        }

        public event EventHandler<CookieCacheReloadedEventArgs> CacheReloaded;
    }
}
