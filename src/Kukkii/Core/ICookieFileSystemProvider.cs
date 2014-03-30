using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieFileSystemProvider
    {
        //byte[] ReadFile(string applicationName, string contextInfo);
        //void SaveFile(string applicationName, string contextInfo, byte[] data);

        Task<byte[]> ReadFileAsync(string applicationName, string contextInfo);
        Task SaveFileAsync(string applicationName, string contextInfo, byte[] data);
    }
}
