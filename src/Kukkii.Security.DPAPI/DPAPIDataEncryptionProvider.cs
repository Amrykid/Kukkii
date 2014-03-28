using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kukkii.Security.DPAPI
{
    public class DPAPIDataEncryptionProvider: ICookieDataEncryptionProvider
    {
        public byte[] EncryptData(byte[] unencryptedData)
        {
            //System.Security.ProtectedData
            throw new NotImplementedException();
        }

        public byte[] DecryptData(byte[] encryptedData)
        {
            throw new NotImplementedException();
        }
    }
}
