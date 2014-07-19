using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace Kukkii.UniversalApps
{
    class UniversalDataEncryptionProvider: Kukkii.Core.ICookieDataEncryptionProvider
    {
        private Windows.Security.Cryptography.DataProtection.DataProtectionProvider provider = null;
        public UniversalDataEncryptionProvider()
        {
            provider = new DataProtectionProvider("LOCAL=user");
        }
        public byte[] EncryptData(byte[] unencryptedData)
        {
            //provider.ProtectAsync(new Windows.Storage.Streams.Buffer(unencryptedData.Length).)
            throw new NotImplementedException();
        }

        public byte[] DecryptData(byte[] encryptedData)
        {
            throw new NotImplementedException();
        }
    }
}
