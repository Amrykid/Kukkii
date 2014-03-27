using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kukkii.Core;
using Newtonsoft.Json;

namespace Kukkii.Mock
{
    internal class FakeFileSystemProvider: ICookieFileSystemProvider
    {
        private byte[] Data = null;
        internal FakeFileSystemProvider()
        {
            var obj = Activator.CreateInstance(CookieRegistration.DefaultCacheType);

            Data = System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        public byte[] ReadFile(string applicationName, string contextInfo)
        {
            return Data;
        }

        public void SaveFile(string applicationName, string contextInfo, byte[] data)
        {
            Data = data;
            return;
        }
    }
}
