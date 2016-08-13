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

        private static ICookieFileSystemProvider localFileSystemProvider = new UniversalApps.UniversalFileSystemProvider(Windows.Storage.ApplicationData.Current.LocalFolder);
        private static ICookieFileSystemProvider localCacheFileSystemProvider = new UniversalApps.UniversalFileSystemProvider(Windows.Storage.ApplicationData.Current.LocalCacheFolder);
        private static ICookieFileSystemProvider roamingFileSystemProvider = new UniversalApps.UniversalFileSystemProvider(Windows.Storage.ApplicationData.Current.RoamingFolder);
        private static ICookieDataEncryptionProvider dataEncryptionProvider = new UniversalApps.UniversalDataEncryptionProvider();
        private static ICookieDataRoamingProvider roamingProvider = new UniversalApps.UniversalRoamingDataProvider();
        private static bool initialized = false;

        public static ICookieFileSystemProvider LocalFileSystemProvider
        {
            get { return localFileSystemProvider ?? new FakeFileSystemProvider(); }
            set { CheckForInitialization(); localFileSystemProvider = value; }
        }

        public static ICookieFileSystemProvider LocalCacheFileSystemProvider
        {
            get { return localCacheFileSystemProvider ?? new FakeFileSystemProvider(); }
            set { CheckForInitialization(); localCacheFileSystemProvider = value; }
        }

        public static ICookieFileSystemProvider RoamingFileSystemProvider
        {
            get { return roamingFileSystemProvider ?? new FakeFileSystemProvider(); }
            set { CheckForInitialization(); roamingFileSystemProvider = value; }
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
