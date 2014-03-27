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

        [DataMember]
        public string Key { get; internal set; }
        [DataMember]
        public T Object { get; internal set; }
        [DataMember]
        public int RequestedExpirationTime { get; internal set; }
        [DataMember]
        public DateTime InsertionTime { get; internal set; }
        internal bool IsExpired()
        {
            return RequestedExpirationTime == -1 ? false : DateTime.Now >= InsertionTime.AddMilliseconds(RequestedExpirationTime);
        }
    }
}
