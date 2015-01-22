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
            if (CookieJarIsInitialized)
                throw new InvalidOperationException(typeof(CookieRegistration).Name + " can't be modified after CookieJar initialization.");
        }

        internal static Type DefaultCacheType { get { return typeof(List<CookieDataPacket<object>>); } }

        private static ICookieFileSystemProvider fileSystemProvider = null;
        private static ICookieDataEncryptionProvider dataEncryptionProvider = null;
        private static ICookieDataRoamingProvider roamingProvider = null;
        private static bool initialized = false;

        public static ICookieFileSystemProvider FileSystemProvider
        {
            get { return fileSystemProvider ?? new FakeFileSystemProvider(); }
            set { CheckForInitialization(); fileSystemProvider = value; }
        }

        public static ICookieDataEncryptionProvider DataEncryptionProvider
        {
            get { return dataEncryptionProvider ?? new FakeDataEncryptionProvider(); }
            set { CheckForInitialization(); dataEncryptionProvider = value; }
        }

        public static ICookieDataRoamingProvider RoamingDataProvider
        {
            get { return roamingProvider ?? new FakeDataRoamingProvider(); }
            set { CheckForInitialization(); roamingProvider = value; }
        }

        public static bool CookieJarIsInitialized { get { return initialized; } internal set { initialized = value; } }
    }
}
