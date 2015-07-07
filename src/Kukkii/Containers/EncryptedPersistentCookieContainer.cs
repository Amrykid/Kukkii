using System;
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
    public class EncryptedPersistentCookieContainer: PersistentCookieContainer
    {
        private ICookieDataEncryptionProvider encryptionProvider = null;
        private bool containerDisabled = false;
        internal EncryptedPersistentCookieContainer(CookieMonster cookie, ICookieFileSystemProvider filesystem, ICookieDataEncryptionProvider encryptor, bool isLocal): base(cookie, filesystem, isLocal)
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
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key contains invalid characters.", "key");

            return _InternalGetObject(key).ContinueWith<T>(task =>
            {
                //if the result was null, call the creation task.
                //otherwise, remove the object from the cache

                if (task.Result == null)
                {
                    if (creationFunction != null)
                        return creationFunction(); //call the creation function to get a replacement item.
                    else
                        return default(T); //nothing else we can do.
                }
                else
                {
                    var cookie = (CookieDataPacket<object>)task.Result;

                    lock (Cache)
                    {
                        //remove the item from the cache
                        Cache.Remove(cookie);
                    }

                    return (T)cookie.Object; //return the unwrapped item/object.
                }
            });
        }

        public override Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null)
        {
            return _InternalGetObject(key).ContinueWith(x =>
                {
                    if (x.Result != null)
                    {
                        var cookie = (CookieDataPacket<object>)x.Result;
                        return (T)cookie.Object;
                    }
                    else
                    {
                        if (creationFunction != null)
                            return creationFunction(); //call the creation function to get a replacement item.
                        else
                            return default(T);
                    }
                });
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

            //load cache from disk

            var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo, base.providerIsLocal);

            if (data != null)
            {
                data = encryptionProvider.DecryptData(data);

                var jsonStr = System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length);

                Cache = JsonConvert.DeserializeObject<IList<CookieDataPacket<object>>>(jsonStr);
            }

            cacheLoaded = true;
        }

        public override Task FlushAsync()
        {
            return WriteDataViaFileSystem(encryptionProvider.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache))));
        }
    }
}
