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
            if (!cacheLoaded)
            {
                await CacheLock.WaitAsync();
                await initializeLock.WaitAsync();

                if (!cacheLoaded) //after waiting for its turn, if the cache /still/ isn't loaded, try again.
                {
                    try
                    {
                        var data = await filesystem.ReadFileAsync(CookieJar.ApplicationName, contextInfo).ConfigureAwait(false);

                        if (data != null)
                        {
                            data = encryptionProvider.DecryptData(data);

                            if (data != null)
                            {
                                LoadCacheFromData(data);
                            }

                            cacheLoaded = true;
                        }
                    }
                    catch (JsonException ex)
                    {
                        initializeLock.Release();
                        CacheLock.Release();

                        throw new CacheCannotBeLoadedException("Unable to load cache.", ex);
                    }
                    catch (Exception ex)
                    {
                        initializeLock.Release();
                        CacheLock.Release();

                        switch (ex.HResult)
                        {
                            case -2146881269: //corrupted encrypted file data
                                throw new CacheCannotBeLoadedException("Unable to load cache.", ex);
                        }
                    }

                    CacheLock.Release();
                    initializeLock.Release();
                }
            }
        }

        public override Task FlushAsync()
        {
            return WriteDataViaFileSystemAsync(encryptionProvider.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache))));
        }
    }
}
