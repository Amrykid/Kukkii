using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kukkii.Core;
using Kukkii.Mock;

namespace Kukkii
{
    public static class CookieRegistration
    {
        private static void CheckForInitialization()
        {
            if (CookieJar.IsInitialized)
                throw new InvalidOperationException(typeof(CookieRegistration).Name + " can't be modified after CookieJar initialization.");
        }

        internal static Type DefaultCacheType { get { return typeof(List<CookieDataPacket<object>>); } }

        private static ICookieFileSystemProvider fileSystemProvider = null;

        public static ICookieFileSystemProvider FileSystemProvider
        {
            get { return fileSystemProvider ?? new FakeFileSystem(); }
            set { CheckForInitialization(); fileSystemProvider = value; }
        }
    }
}
