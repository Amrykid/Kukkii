using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kukkii.Containers
{
    public class PersistentCookieContainer: BasicCookieContainer
    {
        internal PersistentCookieContainer()
        {
            //load cache from disk
        }

        public System.Threading.Tasks.Task<object> GetObjectAsync(string key, Func<object> creationFunction = null)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<IEnumerable<object>> GetObjectsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<object> PeekObjectAsync(string key)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> InsertObjectAsync(string key, object item, int expirationTime = -1)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> CleanUpAsync()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task<bool> FlushAsync()
        {
            //save cache to disk
            throw new NotImplementedException();
        }
    }
}
