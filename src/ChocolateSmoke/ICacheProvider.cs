using System;
using System.Threading.Tasks;

namespace ChocolateSmoke;

/// <summary>
/// Defines a contract for a cache provider that supports asynchronous operations for retrieving, storing, and removing cached data.
/// </summary>
public interface ICacheProvider
{
    /// <summary>
    /// Asynchronously retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached item.</typeparam>
    /// <param name="key">The unique key identifying the cached item.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached item, or default(T) if the key does not exist.</returns>
    Task<T> GetAsync<T>(string key);

    /// <summary>
    /// Asynchronously stores an item in the cache with an optional time-to-live (TTL).
    /// </summary>
    /// <typeparam name="T">The type of the item to cache.</typeparam>
    /// <param name="key">The unique key to associate with the cached item.</param>
    /// <param name="value">The item to cache.</param>
    /// <param name="ttl">An optional time-to-live value. If null, the item does not expire.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);

    /// <summary>
    /// Asynchronously removes a cached item by its key.
    /// </summary>
    /// <param name="key">The unique key identifying the cached item to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);

    /// <summary>
    /// Gets the priority of this cache provider when multiple providers are used.
    /// A lower value indicates a higher priority.
    /// </summary>
    int LookupPriority { get; }
}