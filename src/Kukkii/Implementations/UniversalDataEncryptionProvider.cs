using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Storage.Provider;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Kukkii.UniversalApps
{
    public class UniversalDataEncryptionProvider: Kukkii.Core.ICookieDataEncryptionProvider
    {
        private Windows.Security.Cryptography.DataProtection.DataProtectionProvider provider = null;
        public UniversalDataEncryptionProvider()
        {
            provider = new DataProtectionProvider("LOCAL=user");
        }
        public byte[] EncryptData(byte[] unencryptedData)
        {
            var buffer = unencryptedData.AsBuffer();

            var protectedBuffer = provider.ProtectAsync(buffer).AsTask().Result;

            return protectedBuffer.ToArray();
        }

        public byte[] DecryptData(byte[] encryptedData)
        {
            var buffer = encryptedData.AsBuffer();

            var unprotectedBuffer = provider.UnprotectAsync(buffer).AsTask().Result;

            return unprotectedBuffer.ToArray();
        }
    }
}
