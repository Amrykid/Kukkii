using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kukkii.Containers
{
    public class DataRoamingPersistentCookieContainer : PersistentCookieContainer, IDataRoamingPersistentCookieContainer
    {
        public DataRoamingPersistentCookieContainer(CookieMonster threadRunner, ICookieFileSystemProvider cookieFileSystemProvider, bool p)
            : base(threadRunner, cookieFileSystemProvider, p)
        {

        }

    }
}
