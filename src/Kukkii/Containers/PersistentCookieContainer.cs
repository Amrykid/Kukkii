using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kukkii.Containers
{
    public class PersistentCookieContainer: BasicCookieContainer
    {
        private ICookieFileSystem fileSystemProvider = null;
        private bool cacheLoaded = false;
        internal PersistentCookieContainer(ICookieFileSystem filesystem)
        {
            fileSystemProvider = filesystem;
        }

        private void InitializeCacheIfNotDoneAlready(ICookieFileSystem filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            Cache = (dynamic)filesystem.ReadFile(CookieJar.ApplicationName, "persistent_cache");
            cacheLoaded = true;
        }

        public System.Threading.Tasks.Task<object> GetObjectAsync(string key, Func<object> creationFunction = null)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<IEnumerable<object>> GetObjectsAsync(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<object> PeekObjectAsync(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> InsertObjectAsync(string key, object item, int expirationTime = -1)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> CleanUpAsync()
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.CleanUpAsync();
        }

        public System.Threading.Tasks.Task<bool> FlushAsync()
        {
            //save cache to disk
            throw new NotImplementedException();
        }
    }
}
