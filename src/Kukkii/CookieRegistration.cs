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
        internal static Type DefaultCacheType { get { return typeof(List<KeyValuePair<string, CookieDataPacket<object>>>); } }

        private static ICookieFileSystem fileSystemProvider = null;

        public static ICookieFileSystem FileSystemProvider
        {
            get { return fileSystemProvider ?? new FakeFileSystem(); }
            set { fileSystemProvider = value; }
        }
    }
}
