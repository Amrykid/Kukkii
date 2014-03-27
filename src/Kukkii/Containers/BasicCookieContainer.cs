using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Containers
{
    /// <summary>
    /// Provides a basic data container that does not persist.
    /// </summary>
    public class BasicCookieContainer : ICookieContainer
    {
        public BasicCookieContainer()
        {
            //Create an object to hold all of the items stored in the container.
            Cache = (dynamic)Activator.CreateInstance(CookieRegistration.DefaultCacheType);
        }

        /// <summary>
        /// Grabs an object from the cache using the key provided. The object is removed from the cache. If the object does not exists, the creation function is called to provide a replacement object.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <param name="creationFunction">The function to call to provide a replacement item should the key/item not exist.</param>
        /// <returns></returns>
        public virtual System.Threading.Tasks.Task<object> GetObjectAsync(string key, Func<object> creationFunction = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            return _InternalGetObject(key).ContinueWith<object>(task =>
                {
                    //if the result was null, call the creation task.
                    //otherwise, remove the object from the cache

                    if (task.Result == null)
                    {
                        if (creationFunction != null)
                            return creationFunction(); //call the creation function to get a replacement item.
                        else
                            return null; //nothing else we can do.
                    }
                    else
                    {
                        lock (Cache)
                        {
                            //remove the item from the cache
                            Cache.Remove((KeyValuePair<string,CookieDataPacket<object>>)task.Result);
                        }

                        return ((KeyValuePair<string, CookieDataPacket<object>>)task.Result).Value.Object; //return the unwrapped item/object.
                    }
                });
        }

        public virtual System.Threading.Tasks.Task<IEnumerable<object>> GetObjectsAsync(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the object stored using the provided key and not remove it from the cache.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <returns></returns>
        public virtual System.Threading.Tasks.Task<object> PeekObjectAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            return _InternalGetObject(key).ContinueWith(x => ((KeyValuePair<string, CookieDataPacket<object>>)x.Result).Value.Object);
        }

        /// <summary>
        /// An internal function for getting an object from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private Task<object> _InternalGetObject(string key)
        {
            TaskCompletionSource<object> itemTask = new TaskCompletionSource<object>();

            CookieMonster.QueueWork(() =>
            {
                lock (Cache)
                {
                    var firstResult = Cache.Where(items => items.Key == key)
                        .OrderBy(x => x.Value.InsertionTime)
                        .FirstOrDefault();

                    itemTask.TrySetResult(firstResult);
                }
            });

            return itemTask.Task;
        }

        /// <summary>
        /// Inserts an object into the cache with a key and optional expiration time.
        /// </summary>
        /// <param name="key">The key used to store the object.</param>
        /// <param name="item">The object to store.</param>
        /// <param name="expirationTime">How long (in milliseconds) should the object be fresh. Use -1 for infinity.</param>
        /// <returns></returns>
        public virtual System.Threading.Tasks.Task<bool> InsertObjectAsync(string key, object item, int expirationTime = -1)
        {
            //check the parameters
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");
            if (expirationTime < -1) throw new ArgumentOutOfRangeException("expirationTime");

            return CookieMonster.QueueWork(() =>
                {
                    //creates a cookie wrapper for the object
                    CookieDataPacket<object> cookie = new CookieDataPacket<object>();
                    cookie.Key = key;
                    cookie.RequestedExpirationTime = expirationTime;
                    cookie.Object = item;
                    cookie.InsertionTime = DateTime.Now;

                    lock (Cache) //locks the cache on this thread
                    {
                        //adds the item to the cache
                        Cache.Add(new KeyValuePair<string, CookieDataPacket<object>>(key, cookie));
                    }
                });
        }

        /// <summary>
        /// Clears out the expired items in the cache.
        /// </summary>
        /// <returns></returns>
        public virtual System.Threading.Tasks.Task<bool> CleanUpAsync()
        {
            return CookieMonster.QueueWork(() =>
                {
                    var expiredItems = Cache.Where(x => x.Value.IsExpired()).ToArray(); //Finds all of the expired items.

                    lock (Cache) //locks the cache
                    {
                        foreach (var item in expiredItems)
                            Cache.Remove(item); //removes each expired item from the cache
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
