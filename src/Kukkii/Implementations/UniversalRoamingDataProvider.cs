using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Kukkii.UniversalApps
{
    public class UniversalRoamingDataProvider: ICookieDataRoamingProvider
    {
        private Containers.DataRoamingPersistentCookieContainer.DataRoamingPersistentCookieContainerRemote cookieJarRemote = null;
        public UniversalRoamingDataProvider()
        {
            ApplicationData.Current.DataChanged += Current_DataChanged;
        }

        void Current_DataChanged(ApplicationData sender, object args)
        {
            CookieJar.Roaming.ReloadCacheAsync();

            cookieJarRemote.SignalDataRoamedEvent();
        }

        public void ReceiveRemote(Containers.DataRoamingPersistentCookieContainer.DataRoamingPersistentCookieContainerRemote dataRemote)
        {
            cookieJarRemote = dataRemote;
        }
    }
}
