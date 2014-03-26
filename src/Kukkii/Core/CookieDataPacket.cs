using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Kukkii.Core
{
    [DataContract]
    public class CookieDataPacket<T>
    {
        internal CookieDataPacket()
        {
        }

        internal string Key { get; set; }
        internal T Object { get; set; }
        internal int RequestedExpirationTime { get; set; }
        internal DateTime InsertionTime { get; set; }
        internal bool IsExpired()
        {
            return RequestedExpirationTime == -1 ? false : DateTime.Now >= InsertionTime.AddMilliseconds(RequestedExpirationTime);
        }
    }
}
