using System;
using System.Threading.Tasks;

namespace ChocolateSmoke
{
    /// <summary>
    ///     Cache manager interface that orchestrates the caching operations across multiple providers.
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Registers a cache provider
        /// </summary>
        /// <param name="provider">Instance of <see cref="ICacheProvider"/>.</param>
        /// <returns>Instance of <see cref="CacheManager"/>.</returns>
        CacheManager UseProvider(ICacheProvider provider);

        /// <summary>
        /// Sets the caching strategy to be used by the <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="strategy">
        /// The <see cref="CacheStrategy"/> to apply (e.g., CacheAside, WriteThrough, etc.).
        /// </param>
        /// <returns>
        /// The current <see cref="CacheManager"/> instance to allow method chaining.
        /// </returns>
        CacheManager WithStrategy(CacheStrategy strategy);

        /// <summary>
        /// Enables or disables promotion of cache entries to higher-priority providers when a cache hit occurs.
        /// </summary>
        /// <param name="enable">
        /// If <c>true</c>, promotes the cached item to a higher-level cache on hit; otherwise, disables promotion.
        /// </param>
        /// <returns>
        /// The current <see cref="CacheManager"/> instance to allow method chaining.
        /// </returns>
        CacheManager WithPromotionOnHit(bool enable);

        /// <summary>
        /// Retrieves a value from the cache using the specified key. If the value is not found and a fallback is provided, the fallback function is invoked to retrieve and optionally cache the value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The cache key associated with the value.</param>
        /// <param name="fallback">
        /// An optional asynchronous function to fetch the value from the source if it's not found in the cache.
        /// If provided and the value is retrieved, it may be cached based on the configured strategy.
        /// </param>
        /// <param name="timeToLive">An optional time-to-live value for the cached item. If null, the item does not expire unless the provider enforces a default policy. </param>
        /// <returns>The cached value if found; otherwise, the result of the fallback function or default if none is provided.</returns>
        Task<T> GetAsync<T>(string key, Func<Task<T>> fallback = null, TimeSpan? timeToLive = null);

        /// <summary>
        /// Stores a value in the cache using the specified key. Optionally, writes the value to a backing store if a delegate is provided.
        /// </summary>
        /// <typeparam name="T">The type of the value to store.</typeparam>
        /// <param name="key">The cache key associated with the value.</param>
        /// <param name="value">The value to store in the cache.</param>
        /// <param name="timeToLive">An optional time-to-live value for the cached item. If null, the item does not expire unless the provider enforces a default policy.</param>
        /// <param name="writeToStore">
        /// An optional asynchronous delegate that writes the value to a persistent store (e.g., a database).
        /// If provided, this delegate is executed after caching the value.
        /// </param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null, Func<T, Task> writeToStore = null);

        /// <summary>
        /// Removes the specified key from all registered cache providers.
        /// </summary>
        /// <param name="key">The unique cache key to remove.</param>
        /// <returns>A task that represents the asynchronous removal operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the key is null or empty.</exception>
        Task RemoveAsync(string key);
    }
}