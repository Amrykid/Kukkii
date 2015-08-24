using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;

namespace Kukkii.Containers
{
    public class PersistentCookieContainer : BasicCookieContainer, IPersistentCookieContainer
    {
        protected ICookieFileSystemProvider fileSystemProvider = null;
        protected volatile bool cacheLoaded = false;
        protected string contextInfo = "persistent_cache";
        private JsonSerializer serializer = null;
        protected bool providerIsLocal = false;
        internal PersistentCookieContainer(CookieMonster cookie, ICookieFileSystemProvider filesystem, bool isLocal)
            : base(cookie)
        {
            fileSystemProvider = filesystem;

            providerIsLocal = isLocal;

            serializer = new JsonSerializer();
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            serializer.MaxDepth = 2048;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;

            reloadingTask.TrySetResult(0); //default status
        }

        protected virtual async Task InitializeCacheIfNotDoneAlreadyAsync(ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo, providerIsLocal);

            if (data != null)
            {
                LoadCacheFromData(data);

            }
            cacheLoaded = true;
        }

        protected void LoadCacheFromData(byte[] data)
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

        public override async Task<bool> ContainsObjectAsync(string key)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.ContainsObjectAsync(key);
        }

        public override async Task<int> CountObjectsAsync(string key)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.CountObjectsAsync(key);
        }

        public override async System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.GetObjectAsync<T>(key, creationFunction);
        }

        public override async System.Threading.Tasks.Task<IEnumerable<T>> GetObjectsAsync<T>(string key)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.GetObjectsAsync<T>(key);
        }

        public override async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.PeekObjectAsync<T>(key, creationFunction);
        }

        public override async System.Threading.Tasks.Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.InsertObjectAsync(key, item, expirationTime);
        }

        public override async System.Threading.Tasks.Task CleanUpAsync()
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.CleanUpAsync();
        }
        /// <summary>
        /// Saves the current cache to disk.
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task FlushAsync()
        {
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

            return WriteDataViaFileSystem(System.Text.UTF8Encoding.UTF8.GetBytes(json));
        }

        public override async Task UpdateObjectAsync<T>(string key, T item)
        {
            await reloadingTask.Task;

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.UpdateObjectAsync<T>(key, item);
        }

        protected Task WriteDataViaFileSystem(byte[] data)
        {
            return CookieMonster.QueueWork(() =>
            {
                fileSystemProvider.SaveFileAsync(CookieJar.ApplicationName, contextInfo, data, providerIsLocal).Wait();

                return true;
            });
        }

        private TaskCompletionSource<object> reloadingTask = new TaskCompletionSource<object>();

        public async Task ReloadCacheAsync()
        {
            if (!cacheLoaded || fileSystemProvider == null) throw new InvalidOperationException();

            if (reloadingTask.Task.IsCompleted)
                reloadingTask = new TaskCompletionSource<object>();

            await CookieMonster.QueueWork(() => null); //wait for the queue to empty

            cacheLoaded = false;
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            reloadingTask.TrySetResult(0);

            if (CacheReloaded != null)
                CacheReloaded(this, new CookieCacheReloadedEventArgs());
        }

        public bool IsCacheLoaded
        {
            get { return cacheLoaded; }
        }


        public async Task InitializeAsync()
        {
            if (!reloadingTask.Task.Wait(1))
                throw new InvalidOperationException();

            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);
        }

        public event EventHandler<CookieCacheReloadedEventArgs> CacheReloaded;
    }
}
