using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kukkii.Core;

namespace Kukkii.Mock
{
    internal class FakeFileSystem: ICookieFileSystem
    {
        public object ReadFile(string applicationName, string contextInfo)
        {
            return Activator.CreateInstance(CookieRegistration.DefaultCacheType);
        }

        public void SaveFile(string applicationName, string contextInfo, object data)
        {
            return;
        }
    }
}
