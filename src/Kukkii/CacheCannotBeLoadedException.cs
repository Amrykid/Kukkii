using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii
{
    public class CacheCannotBeLoadedException: Exception
    {
        internal CacheCannotBeLoadedException(string message): base(message)
        {

        }
        internal CacheCannotBeLoadedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
