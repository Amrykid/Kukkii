﻿using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kukkii.Containers
{
    /// <summary>
    /// Provides a basic data container that does not persist.
    /// </summary>
    public class BasicCookieContainer : ICookieContainer
    {
        protected SemaphoreSlim CacheLock { get; set; } = new SemaphoreSlim(1);
        public BasicCookieContainer()
        {
            //Create an object to hold all of the items stored in the container.
            Cache = (List<CookieDataPacket<object>>)Activator.CreateInstance(CookieRegistration.DefaultCacheType);
        }

        /// <summary>
        /// Grabs an object from the container using the key provided. The object is removed from the container. If the object does not exists, the creation function is called to provide a replacement object.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <param name="creationFunction">The function to call to provide a replacement item should the key/item not exist.</param>
        /// <returns></returns>
        public virtual async System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            await CacheLock.WaitAsync();

            T returnValue;


            var dataPacket = Cache.Where(x => x.Key == key)
                .OrderBy(x => x.InsertionTime)
                .FirstOrDefault();

            if (dataPacket != null ? dataPacket.Object != null : false)
            {
                lock (Cache)
                {
                    //remove the item from the cache
                    Cache.Remove(dataPacket);
                }

                returnValue = (T)(dataPacket.Object); //return the unwrapped item/object.
            }
            else
            {
                if (creationFunction != null)
                    returnValue = creationFunction(); //call the creation function to get a replacement item.
                else
                    returnValue = default(T); //nothing else we can do.
            }

            CacheLock.Release();

            return returnValue;
        }

        public virtual async System.Threading.Tasks.Task<IEnumerable<T>> GetObjectsAsync<T>(string key)
        {
            await CacheLock.WaitAsync();

            List<T> objects = new List<T>();

            T item;

            while ((item = await GetObjectAsync<T>(key)) != null)
                objects.Add(item);

            CacheLock.Release();

            return objects;
        }

        /// <summary>
        /// Returns the object stored using the provided key and not remove it from the container.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <returns></returns>
        public virtual async System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            await CacheLock.WaitAsync();

            T returnValue;


            var dataPacket = Cache.Where(x => x.Key == key)
                .OrderBy(x => x.InsertionTime)
                .FirstOrDefault();

            if (dataPacket != null ? dataPacket.Object != null : false)
            {
                returnValue = (T)dataPacket.Object;
            }
            else
            {
                if (creationFunction != null)
                    returnValue = creationFunction(); //call the creation function to get a replacement item.
                else
                    returnValue = default(T);
            }

            CacheLock.Release();

            return returnValue;
        }

        /// <summary>
        /// Inserts an object into the container with a key and optional expiration time.
        /// </summary>
        /// <param name="key">The key used to store the object.</param>
        /// <param name="item">The object to store.</param>
        /// <param name="expirationTime">How long (in milliseconds) should the object be fresh. Use -1 for infinity.</param>
        /// <returns></returns>
        public virtual System.Threading.Tasks.Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1)
        {
            //check the parameters
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");
            if (expirationTime < -1) throw new ArgumentOutOfRangeException("expirationTime");

            CookieDataPacket<object> cookie = CreateCookiePacket<T>(key, item, expirationTime);

            return AddCookiePacketToCache(cookie);
        }

        protected async Task AddCookiePacketToCache(CookieDataPacket<object> cookie)
        {
            await CacheLock.WaitAsync();

            //adds the item to the cache
            Cache.Add(cookie);

            CacheLock.Release();
        }

        protected CookieDataPacket<object> CreateCookiePacket<T>(string key, T item, int expirationTime)
        {
            //creates a cookie wrapper for the object
            CookieDataPacket<object> cookie = new CookieDataPacket<object>();
            cookie.Key = key;
            cookie.RequestedExpirationTime = expirationTime;
            cookie.Object = item;
            cookie.InsertionTime = DateTime.Now;
            return cookie;
        }

        /// <summary>
        /// Clears out the expired items in the container.
        /// </summary>
        /// <returns></returns>
        public virtual async System.Threading.Tasks.Task CleanUpAsync()
        {
            await CacheLock.WaitAsync();

            var expiredItems = Cache.Where(x => x.IsExpired()).ToArray(); //Finds all of the expired items.

            foreach (var item in expiredItems)
                Cache.Remove(item); //removes each expired item from the cache

            CacheLock.Release();

        }

        public virtual System.Threading.Tasks.Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        protected IList<CookieDataPacket<object>> Cache { get; set; }

        /// <summary>
        /// Checks the container for an object with the specified key.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns></returns>
        public virtual async Task<bool> ContainsObjectAsync(string key)
        {
            await CacheLock.WaitAsync();

            bool returnValue = Cache.Any(x => x.Key == key);

            CacheLock.Release();
            return returnValue;

        }

        /// <summary>
        /// Deletes everything from the container.
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearContainerAsync()
        {
            await CacheLock.WaitAsync();

            Cache.Clear();

            CacheLock.Release();
        }


        public async virtual Task UpdateObjectAsync<T>(string key, T item)
        {
            //todo handle race condition.
            if (!await ContainsObjectAsync(key))
            {
                await InsertObjectAsync<T>(key, item);
            }
            else
            {

                await CacheLock.WaitAsync();

                //remove the item from the cache

                var oldItem = Cache.First(x => x.Key == key);
                var index = Cache.IndexOf(oldItem);

                CookieDataPacket<object> cookie = CreateCookiePacket<T>(key, item, oldItem.RequestedExpirationTime);

                Cache[index] = cookie;

                CacheLock.Release();

            }
        }


        public virtual async Task<int> CountObjectsAsync(string key)
        {
            await CacheLock.WaitAsync();

            int returnValue = Cache.Where(x => x.Key == key).Count();

            CacheLock.Release();

            return returnValue;
        }
    }
}
