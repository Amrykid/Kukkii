﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface IPersistentCookieContainer: ICookieContainer
    {
        bool IsCacheLoaded { get; }

        Task ReloadCacheAsync();
    }
}
