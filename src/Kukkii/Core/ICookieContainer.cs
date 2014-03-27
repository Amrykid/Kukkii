using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieContainer : ICookieContainer<object>
    { }

    public interface ICookieContainer<T>
    {
        Task<bool> ContainsObjectAsync(string key);
        Task<T> GetObjectAsync(string key, Func<T> creationFunction = null);
        Task<IEnumerable<T>> GetObjectsAsync(string key);
        Task<T> PeekObjectAsync(string key);
        Task InsertObjectAsync(string key, object item, int expirationTime = -1);

        Task CleanUpAsync();
        Task ClearContainerAsync();
        Task FlushAsync();
    }
}
