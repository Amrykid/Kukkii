using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieContainer
    {
        Task<bool> ContainsObjectAsync(string key);
        Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null);
        Task<IEnumerable<T>> GetObjectsAsync<T>(string key);
        Task<T> PeekObjectAsync<T>(string key);
        Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1);

        Task CleanUpAsync();
        Task ClearContainerAsync();
        Task FlushAsync();
    }
}
