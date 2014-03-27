using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.UnitTests
{
    internal static class Utilities
    {
        internal static void ForcePersistentCacheReload()
        {
            //use reflection to reset the container
            var containerType = CookieJar.Device.GetType();
            var cacheLoadedFieldInfo = containerType.GetField("cacheLoaded", BindingFlags.Instance | BindingFlags.NonPublic);
            cacheLoadedFieldInfo.SetValue(CookieJar.Device, false); //tells the container that it has not loaded the cache to force it to reload the cache.
        }

        internal static void ForceResetCookieJar(bool reinitialize = true)
        {
            var jarType = typeof(CookieJar);
            var isInitializedFieldInfo = jarType.GetProperty("IsInitialized", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            isInitializedFieldInfo.SetValue(null, false);


            if (reinitialize)
            {
                var initializeFieldInfo = jarType.GetMethod("Initialize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                initializeFieldInfo.Invoke(null, new object[] { });
            }
        }
    }
}
