using System.Threading.Tasks;

namespace ChocolateSmoke
{
    /// <summary>
    /// Represents a cache provider that supports the Read-Through caching strategy,
    /// where the provider is responsible for loading data from the source on a cache miss.
    /// </summary>
    public interface IReadThroughCacheProvider : ICacheProvider
    {
        /// <summary>
        /// Retrieves an item from the cache or loads it from the underlying data source if not found.
        /// </summary>
        /// <typeparam name="T">The type of the item to retrieve.</typeparam>
        /// <param name="key">The unique key identifying the cached item.</param>
        /// <returns>
        /// The cached item if found; otherwise, the item loaded from the data source.
        /// </returns>
        Task<T> GetOrLoadAsync<T>(string key);
    }
}