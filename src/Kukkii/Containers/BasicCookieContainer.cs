using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Containers
{
    public class BasicCookieContainer : ICookieContainer
    {
        public BasicCookieContainer()
        {
            Cache = new List<KeyValuePair<string, CookieDataPacket<object>>>();
        }

        public virtual System.Threading.Tasks.Task<object> GetObjectAsync(string key, Func<object> creationFunction = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            return PeekObjectAsync(key).ContinueWith<object>(task =>
                {
                    //if the result was null, call the creation task.
                    //otherwise, remove the object from the cache

                    if (task.Result == null)
                    {
                        if (creationFunction != null)
                            return creationFunction();
                        else
                            return null; //nothing else we can do.
                    }
                    else
                    {
                        lock (Cache)
                        {
                            Cache.Remove((KeyValuePair<string,CookieDataPacket<object>>)task.Result);
                        }
                    }

                    return null;
                });
        }

        public virtual System.Threading.Tasks.Task<IEnumerable<object>> GetObjectsAsync(string key)
        {
            throw new NotImplementedException();
        }

        public virtual System.Threading.Tasks.Task<object> PeekObjectAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            TaskCompletionSource<object> itemTask = new TaskCompletionSource<object>();
            
            CookieMonster.QueueWork(() =>
            {
                lock (Cache)
                {
                    itemTask.TrySetResult(Cache.Where(items => items.Key == key)
                        .OrderBy(x => x.Value.InsertionTime)
                        .FirstOrDefault());
                }
            });

            return itemTask.Task;
        }

        public virtual System.Threading.Tasks.Task<bool> InsertObjectAsync(string key, object item, int expirationTime = -1)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");
            if (expirationTime < -1) throw new ArgumentOutOfRangeException("expirationTime");

            return CookieMonster.QueueWork(() =>
                {
                    CookieDataPacket<object> cookie = new CookieDataPacket<object>();
                    cookie.Key = key;
                    cookie.RequestedExpirationTime = expirationTime;
                    cookie.Object = item;
                    cookie.InsertionTime = DateTime.Now;

                    lock (Cache)
                    {
                        Cache.Add(new KeyValuePair<string, CookieDataPacket<object>>(key, cookie));
                    }
                });
        }

        public virtual System.Threading.Tasks.Task<bool> CleanUpAsync()
        {
            return CookieMonster.QueueWork(() =>
                {
                    var expiredItems = Cache.Where(x => x.Value.IsExpired()).ToArray();

                    lock (Cache)
                    {
                        foreach (var item in expiredItems)
                            Cache.Remove(item);
                    }

                });
        }

        public virtual System.Threading.Tasks.Task<bool> FlushAsync()
        {
            throw new NotImplementedException();
        }

        protected IList<KeyValuePair<string, CookieDataPacket<object>>> Cache { get; set; }
    }
}
