using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kukkii.Core
{
    public interface ICookieContainer
    {
        /// <summary>
        /// Checks the container for an object with the specified key.
        /// </summary>
        /// <param name="key">The key to check against.</param>
        /// <returns></returns>
        Task<bool> ContainsObjectAsync(string key);
        /// <summary>
        /// Grabs an object from the container using the key provided. The object is removed from the container. 
        /// If the object does not exists, the creation function is called to provide a replacement object.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <param name="creationFunction">The function to call to provide a replacement item should the key/item not exist.</param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string key, Func<T> creationFunction = null);
        Task<IEnumerable<T>> GetObjectsAsync<T>(string key);
        /// <summary>
        /// Returns the object stored using the provided key and not remove it from the container. 
        /// If the object does not exists, the creation function is called to provide a replacement object.
        /// </summary>
        /// <param name="key">The key used to locate the object.</param>
        /// <param name="creationFunction">The function to call to provide a replacement item should the key/item not exist.</param>
        /// <returns></returns>
        Task<T> PeekObjectAsync<T>(string key, Func<T> creationFunction = null);
        /// <summary>
        /// Inserts an object into the container with a key and optional expiration time.
        /// </summary>
        /// <param name="key">The key used to store the object.</param>
        /// <param name="item">The object to store.</param>
        /// <param name="expirationTime">How long (in milliseconds) should the object be fresh. Use -1 for infinity.</param>
        /// <returns></returns>
        Task InsertObjectAsync<T>(string key, T item, int expirationTime = -1);

        Task UpdateObjectAsync<T>(string key, T item);

        Task<int> CountObjectsAsync(string key);

        /// <summary>
        /// Clears out the expired items in the container.
        /// </summary>
        /// <returns></returns>
        Task CleanUpAsync();
        /// <summary>
        /// Deletes everything from the container.
        /// </summary>
        /// <returns></returns>
        Task ClearContainerAsync();
        /// <summary>
        /// Saves the current cache to disk.
        /// </summary>
        /// <returns></returns>
        Task FlushAsync();
    }
}
