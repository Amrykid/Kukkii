﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kukkii.Core;
using Kukkii.Mock;
using Newtonsoft.Json;
//using System.Security.Cryptography;

namespace Kukkii.Containers
{
    public class EncryptedPersistentCookieContainer : PersistentCookieContainer
    {
        protected ICookieDataEncryptionProvider encryptionProvider = null;
        private bool containerDisabled = false;
        internal EncryptedPersistentCookieContainer(ICookieFileSystemProvider filesystem, ICookieDataEncryptionProvider encryptor, bool isLocal) : base(filesystem, isLocal)
        {
            contextInfo = "encrypted_persistent_cache";

            if (encryptor is FakeDataEncryptionProvider)
                containerDisabled = true;

            encryptionProvider = encryptor;
        }

        //public override Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1)
        //{
        //    //check the parameters
        //    if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");
        //    if (expirationTime < -1) throw new ArgumentOutOfRangeException("expirationTime");

        //    CookieDataPacket<object> cookie = CreateCookiePacket<T>(key, item, expirationTime);

        //    //cache the cookie packet and encrypt it before adding it to the container cache.
        //    cookie.Object = encryptionProvider.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cookie.Object)));

        //    return AddCookiePacketToCache(cookie);
        //}

        public override Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            return base.GetObjectAsync<T>(key, creationFunction);
        }

        public override Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            return base.PeekObjectAsync<T>(key, creationFunction);
        }

        private T DecryptAndConvertCookieObject<T>(CookieDataPacket<object> cookie)
        {
            var encryptedObject = (byte[])cookie.Object;

            var decryptedBytes = encryptionProvider.DecryptData((byte[])cookie.Object);

            return JsonConvert.DeserializeObject<T>(System.Text.UTF8Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length));
        }

        protected override async Task InitializeCacheIfNotDoneAlreadyAsync(Core.ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            await CacheLock.WaitAsync();

            //load cache from disk


            try
            {
                var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo);

                if (data != null)
                {
                    data = encryptionProvider.DecryptData(data);

                    var jsonStr = System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length);

                    Cache = JsonConvert.DeserializeObject<IList<CookieDataPacket<object>>>(jsonStr);

                    cacheLoaded = true;
                }
            }
            catch (JsonException ex)
            {
                throw new CacheCannotBeLoadedException("Unable to load cache.", ex);
            }
            catch (Exception ex)
            {
                switch(ex.HResult)
                {
                    case -2146881269: //corrupted encrypted file data
                        throw new CacheCannotBeLoadedException("Unable to load cache.", ex);
                }
            }

            CacheLock.Release();
        }

        public override Task FlushAsync()
        {
            return WriteDataViaFileSystemAsync(encryptionProvider.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache))));
        }
    }
}
