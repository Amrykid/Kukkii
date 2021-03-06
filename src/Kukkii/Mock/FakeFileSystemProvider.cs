﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kukkii.Core;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kukkii.Mock
{
    internal class FakeFileSystemProvider: ICookieFileSystemProvider
    {
        private byte[] Data = null;
        internal FakeFileSystemProvider()
        {
            var obj = Activator.CreateInstance(CookieRegistration.DefaultCacheType);

            Data = System.Text.UTF8Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None));
        }

        public Task DeleteFileAsync(string applicationName, string contextInfo)
        {
            return Task.FromResult<object>(null);
        }

        public System.Threading.Tasks.Task<byte[]> ReadFileAsync(string applicationName, string contextInfo)
        {
            return Task.FromResult(Data);
        }

        public System.Threading.Tasks.Task SaveFileAsync(string applicationName, string contextInfo, byte[] data)
        {
            Data = data;
            return Task.Delay(0);
        }
    }
}
