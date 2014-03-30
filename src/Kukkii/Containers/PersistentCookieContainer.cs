using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kukkii.Containers
{
    public class PersistentCookieContainer : BasicCookieContainer
    {
        private ICookieFileSystemProvider fileSystemProvider = null;
        protected bool cacheLoaded = false;
        protected string contextInfo = "persistent_cache";
        internal PersistentCookieContainer(ICookieFileSystemProvider filesystem)
        {
            fileSystemProvider = filesystem;
        }

        protected virtual async Task InitializeCacheIfNotDoneAlreadyAsync(ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo);

            if (data != null)
            {
                Cache = JsonConvert.DeserializeObject<IList<CookieDataPacket<object>>>(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length));
            }

            cacheLoaded = true;
        }

        public override async System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.GetObjectAsync<T>(key, creationFunction);
        }

        public override async System.Threading.Tasks.Task<IEnumerable<T>> GetObjectsAsync<T>(string key)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override async System.Threading.Tasks.Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            await base.InsertObjectAsync(key, item, expirationTime);
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
        public override System.Threading.Tasks.Task FlushAsync()
        {
            //save cache to disk

            return WriteDataViaFileSystem(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache)));
        }

        protected Task WriteDataViaFileSystem(byte[] data)
        {
            return CookieMonster.QueueWork(() =>
            {
                fileSystemProvider.SaveFileAsync(CookieJar.ApplicationName, contextInfo, data).Wait();

                return true;
            });
        }
    }
}
