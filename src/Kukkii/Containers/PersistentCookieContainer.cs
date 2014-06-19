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
        private ICookieFileSystemProvider fileSystemProvider = null;
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
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            serializer.MaxDepth = 2048;
            serializer.TypeNameHandling = TypeNameHandling.All;
            serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;
        }

        protected virtual async Task InitializeCacheIfNotDoneAlreadyAsync(ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo, providerIsLocal);

            if (data != null)
            {
                using (StringReader sr = new StringReader(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length)))
                {
                    using (JsonTextReader jtr = new JsonTextReader(sr))
                    {
                        lock (Cache)
                        {
                            Cache = serializer.Deserialize<IList<CookieDataPacket<object>>>(jtr);
                        }
                    }

                }

            }
            cacheLoaded = true;
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

        public override async System.Threading.Tasks.Task<IEnumerable<T>> GetObjectsAsync<T>(string key)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);

            return await base.PeekObjectAsync<T>(key, creationFunction);
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

        public async Task ReloadCacheAsync()
        {
            if (!cacheLoaded || fileSystemProvider == null) throw new InvalidOperationException();

            await CookieMonster.DeinitializeAsync();
            try
            {
                CookieMonster.Dispose();
            }
            catch (Exception) { }
            cacheLoaded = false;
            CookieMonster = new CookieMonster();
            await InitializeCacheIfNotDoneAlreadyAsync(fileSystemProvider);
        }
    }
}
