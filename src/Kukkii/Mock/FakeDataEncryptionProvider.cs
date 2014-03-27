using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kukkii.Core;

namespace Kukkii.Mock
{
    internal class FakeDataEncryptionProvider: ICookieDataEncryptionProvider
    {
        internal FakeDataEncryptionProvider()
        {
        }

        public byte[] EncryptData(byte[] unencryptedData)
        {
            return unencryptedData;
        }

        public byte[] DecryptData(byte[] encryptedData)
        {
            return encryptedData;
        }
    }
}
