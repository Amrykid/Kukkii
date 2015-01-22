using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface IDataRoamingPersistentCookieContainer: IPersistentCookieContainer
    {
        event EventHandler<CookieDataRoamedEventArgs> DataRoamed;
    }
}
