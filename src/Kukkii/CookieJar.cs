using Kukkii.Containers;
using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii
{
    public static class CookieJar
    {
        static CookieJar()
        {
            //initialize the cookie jar!
            InMemory = new BasicCookieContainer();
            Device = new PersistentCookieContainer();
        }

        public static string ApplicationName { get; set; }
        public static ICookieContainer InMemory { get; private set; }
        public static ICookieContainer Device { get; private set; }
    }
}
