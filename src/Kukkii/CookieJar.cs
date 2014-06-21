using Kukkii.Containers;
using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii
{
    /// <summary>
    /// An object for storing objects as cookies!
    /// </summary>
    public static class CookieJar
    {
        private static CookieMonster threadRunner = null;

        static CookieJar()
        {
            Initialize();
        }

        public static void Initialize()
        {
            //initialize the cookie jar!
            if (!IsInitialized)
            {
                if (threadRunner != null) return;

                threadRunner = new CookieMonster();
                InMemory = new BasicCookieContainer(threadRunner);
                Device = new PersistentCookieContainer(threadRunner, CookieRegistration.FileSystemProvider, true);
                Roaming = new PersistentCookieContainer(threadRunner, CookieRegistration.FileSystemProvider, false);
                //Secure = new EncryptedPersistentCookieContainer(CookieRegistration.FileSystemProvider, CookieRegistration.DataEncryptionProvider);
                IsInitialized = true;
            }
        }

        public static async Task DeinitializeAsync(bool flush = true)
        {
            if (IsInitialized)
            {
                if (flush)
                {
                    await Task.WhenAll(Device.FlushAsync(), Roaming.FlushAsync());
                }
                await threadRunner.DeinitializeAsync();

                InMemory = null;
                Device = null;
                Roaming = null;

                threadRunner = null;

                IsInitialized = false;
            }
        }

        public static bool IsInitialized { get { return CookieRegistration.CookieJarIsInitialized; } internal set { CookieRegistration.CookieJarIsInitialized = value; } }

        /// <summary>
        /// The name of the application using the CookieJar. This is for data persisting purposes.
        /// </summary>
        public static string ApplicationName { get; set; }

        /// <summary>
        /// A cookie container that retains objects for as long as the application is running.
        /// </summary>
        public static ICookieContainer InMemory { get; private set; }
        /// <summary>
        /// A cookie container that can save and load objects on physical media.
        /// </summary>
        public static IPersistentCookieContainer Device { get; private set; }
        public static IPersistentCookieContainer Roaming { get; private set; }
        public static ICookieContainer Secure { get; private set; }
    }
}
