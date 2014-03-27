using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kukkii.Core;
//using System.Security.Cryptography;

namespace Kukkii.Containers
{
    public class EncryptedPersistentCookieContainer: PersistentCookieContainer
    {
        internal EncryptedPersistentCookieContainer(ICookieFileSystem filesystem): base(filesystem)
        {
        }

        protected override void InitializeCacheIfNotDoneAlready(Core.ICookieFileSystem filesystem)
        {
            //base.InitializeCacheIfNotDoneAlready(filesystem);
        }

        public override Task<bool> FlushAsync()
        {
            //return WriteDataViaFileSystem(ProtectedData.
            throw new NotImplementedException();
        }
    }
}
