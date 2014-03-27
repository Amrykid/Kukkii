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
        internal EncryptedPersistentCookieContainer(ICookieFileSystemProvider filesystem, ICookieDataEncryptionProvider encryptor): base(filesystem)
        {
            contextInfo = "encrypted_persistent_cache";

            if (encryptor is FakeDataEncryptionProvider)
                containerDisabled = true;

            encryptionProvider = encryptor;
        }

        protected override void InitializeCacheIfNotDoneAlready(Core.ICookieFileSystemProvider filesystem)
        {
            if (cacheLoaded) return;

            //load cache from disk

            var data = filesystem.ReadFile(CookieJar.ApplicationName, contextInfo);

            if (data != null)
            {
                data = encryptionProvider.DecryptData(data);
                Cache = JsonConvert.DeserializeObject<IList<CookieDataPacket<object>>>(System.Text.UTF8Encoding.UTF8.GetString(data, 0, data.Length));
            }

            cacheLoaded = true;
        }

        public override Task<bool> FlushAsync()
        {
            return WriteDataViaFileSystem(encryptionProvider.EncryptData(System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Cache))));
        }
    }
}
