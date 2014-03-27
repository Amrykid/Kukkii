using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kukkii.Core;
using Newtonsoft.Json;

namespace Kukkii.Mock
{
    internal class FakeFileSystem: ICookieFileSystemProvider
    {
        public byte[] ReadFile(string applicationName, string contextInfo)
        {
            var obj = Activator.CreateInstance(CookieRegistration.DefaultCacheType);

            return System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        public void SaveFile(string applicationName, string contextInfo, byte[] data)
        {
            return;
        }
    }
}
