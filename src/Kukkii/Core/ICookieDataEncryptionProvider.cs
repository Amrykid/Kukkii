using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieDataEncryptionProvider
    {
        byte[] EncryptData(byte[] unencryptedData);
        byte[] DecryptData(byte[] encryptedData);
    }
}
