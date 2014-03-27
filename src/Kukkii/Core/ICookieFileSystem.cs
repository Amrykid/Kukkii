using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieFileSystem
    {
        byte[] ReadFile(string applicationName, string contextInfo);
        void SaveFile(string applicationName, string contextInfo, byte[] data);
    }
}
