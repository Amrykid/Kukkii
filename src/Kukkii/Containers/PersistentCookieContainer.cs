﻿using Kukkii.Core;
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

        protected virtual void InitializeCacheIfNotDoneAlready(ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = filesystem.ReadFile(CookieJar.ApplicationName, contextInfo);

            if (data != null)
            {
                Cache = JsonConvert.DeserializeObject<IList<CookieDataPacket<object>>>(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length));
            }

            cacheLoaded = true;
        }

        public override System.Threading.Tasks.Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.GetObjectAsync<T>(key, creationFunction);
        }

        public override System.Threading.Tasks.Task<IEnumerable<T>> GetObjectsAsync<T>(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task<T> PeekObjectAsync<T>(string key)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            throw new NotImplementedException();
        }

        public override System.Threading.Tasks.Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1)
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.InsertObjectAsync(key, item, expirationTime);
        }

        public override System.Threading.Tasks.Task CleanUpAsync()
        {
            InitializeCacheIfNotDoneAlready(fileSystemProvider);

            return base.CleanUpAsync();
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
                fileSystemProvider.SaveFile(CookieJar.ApplicationName, contextInfo, data);

                return true;
            });
        }
    }
}
