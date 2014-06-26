using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface IPersistentCookieContainer: ICookieContainer
    {
        bool IsCacheLoaded { get; }

        /// <summary>
        /// By default, this is called implicitly whenever you try to access the objects within the container.
        /// This method explicitly initializes the container. You shouldn't have to call this unless you want to load data during a splash screen or something.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        Task ReloadCacheAsync();
    }
}
