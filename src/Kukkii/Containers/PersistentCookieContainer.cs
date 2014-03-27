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
        private ICookieFileSystem fileSystemProvider = null;
        private bool cacheLoaded = false;
        protected string contextInfo = "persistent_cache";
        internal PersistentCookieContainer(ICookieFileSystem filesystem)
        {
            fileSystemProvider = filesystem;
        }

        private void InitializeCacheIfNotDoneAlready(ICookieFileSystem filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = filesystem.ReadFile(CookieJar.ApplicationName, contextInfo);

            if (data != null)
            {
                Cache = JsonConvert.DeserializeObject<List<KeyValuePair<string, CookieDataPacket<object>>>>(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length));
            }

            cacheLoaded = true;
        }

        public override System.Threading.Tasks.Task<object> GetObjectAsync(string key, Func<object> creationFunction = null)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<IEnumerable<object>> GetObjectsAsync(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<object> PeekObjectAsync(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<bool> InsertObjectAsync(string key, object item, int expirationTime = -1)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.InsertObjectAsync(key, item, expirationTime);
        }

        public override System.Threading.Tasks.Task<bool> CleanUpAsync()
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.CleanUpAsync();
        }

        public override System.Threading.Tasks.Task<bool> FlushAsync()
        {
            //save cache to disk

            return CookieMonster.QueueWork(() =>
                {
                    fileSystemProvider.SaveFile(CookieJar.ApplicationName, contextInfo, System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache)));
                });
        }
    }
}
