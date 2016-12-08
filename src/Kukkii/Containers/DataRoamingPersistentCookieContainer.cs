using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kukkii.Containers
{
    public class DataRoamingPersistentCookieContainer : PersistentCookieContainer, IDataRoamingPersistentCookieContainer
    {
        private DataRoamingPersistentCookieContainerRemote dataRemote = null;

        public DataRoamingPersistentCookieContainer(ICookieFileSystemProvider cookieFileSystemProvider, ICookieDataRoamingProvider cookieRoamingProvider, bool isLocal = false)
            : base(cookieFileSystemProvider, isLocal)
        {
            dataRemote = new DataRoamingPersistentCookieContainerRemote();

            if (cookieRoamingProvider == null) throw new ArgumentNullException("cookieRoamingProvider");
            cookieRoamingProvider.ReceiveRemote(dataRemote);
        }

        internal void SignalDataRoamedEvent()
        {
            if (DataRoamed != null)
                DataRoamed(this, new CookieDataRoamedEventArgs());
        }

        public class DataRoamingPersistentCookieContainerRemote
        {
            internal DataRoamingPersistentCookieContainerRemote()
            {

            }

            public void SignalDataRoamedEvent()
            {
                (CookieJar.Roaming as DataRoamingPersistentCookieContainer).SignalDataRoamedEvent();
            }
        }

        public event EventHandler<CookieDataRoamedEventArgs> DataRoamed;
    }
}
