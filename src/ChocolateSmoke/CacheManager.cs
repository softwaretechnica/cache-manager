using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChocolateSmoke
{
    /// <summary>
    /// Implements the cache manager - a controller pattern that is used to orchestrate the necessary actions to achieve the desired result.
    /// </summary>
    public class CacheManager : ICacheManager
    {
        private readonly List<ICacheProvider> _providers;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks;
        private readonly ILogger<CacheManager> _logger;

        private CacheStrategy _strategy;
        private bool _promoteOnHit;

        /// <summary>
        /// Constructor
        /// </summary>
        public CacheManager(ILogger<CacheManager> logger)
        {
            _providers = new List<ICacheProvider>();
            _keyLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _strategy = CacheStrategy.CacheAside;
            _promoteOnHit = false;
        }

        /// <summary>
        /// Registers a cache provider
        /// </summary>
        /// <param name="provider">Instance of <see cref="ICacheProvider"/>.</param>
        /// <returns>Instance of <see cref="CacheManager"/>.</returns>
        public CacheManager UseProvider(ICacheProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider), "Cache provider cannot be null.");
            }
            _providers.Add(provider);
            
            return this;
        }

        /// <summary>
        /// Sets the caching strategy to be used by the <see cref="CacheManager"/>.
        /// </summary>
        /// <param name="strategy">
        /// The <see cref="CacheStrategy"/> to apply (e.g., CacheAside, WriteThrough, etc.).
        /// </param>
        /// <returns>
        /// The current <see cref="CacheManager"/> instance to allow method chaining.
        /// </returns>
        public CacheManager WithStrategy(CacheStrategy strategy)
        {
            _strategy = strategy;
            return this;
        }

        /// <summary>
        /// Enables or disables promotion of cache entries to higher-priority providers when a cache hit occurs.
        /// </summary>
        /// <param name="enable">
        /// If <c>true</c>, promotes the cached item to a higher-level cache on hit; otherwise, disables promotion.
        /// </param>
        /// <returns>
        /// The current <see cref="CacheManager"/> instance to allow method chaining.
        /// </returns>
        public CacheManager WithPromotionOnHit(bool enable)
        {
            _promoteOnHit = enable;
            return this;
        }

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
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> fallback = null, TimeSpan? timeToLive = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
            }
            
            _logger.LogDebug("Getting key '{Key}' using strategy {Strategy}", key, _strategy);

            switch (_strategy)
            {
                case CacheStrategy.CacheAside:
                case CacheStrategy.WriteThrough:
                {
                    return await HandleCacheAside<T>(key, fallback, timeToLive);
                }
                case CacheStrategy.PassThrough:
                {
                    return fallback != null ? await fallback() : default;
                }
                case CacheStrategy.ReadThrough:
                {
                    return await HandleReadThrough<T>(key);
                } 
                default:
                {
                    throw new NotImplementedException($"Strategy {_strategy} not implemented for GetAsync.");
                }
            }
        }

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
        public async Task SetAsync<T>(string key, T value, TimeSpan? timeToLive = null, Func<T, Task> writeToStore = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
            }

            _logger.LogDebug("Setting key '{Key}' using strategy {Strategy}", key, _strategy);

            switch (_strategy)
            {
                case CacheStrategy.WriteThrough:
                {
                    if (writeToStore != null)
                    {
                        await writeToStore(value);
                    }

                    foreach (var provider in _providers)
                    {
                        await provider.SetAsync(key, value);
                    }
                }
                break;
                case CacheStrategy.WriteAround:
                {
                    if (writeToStore != null)
                    {
                        await writeToStore(value);
                    }
                }
                break;
                default:
                {
                    foreach (var provider in _providers)
                    {
                        await provider.SetAsync(key, value);
                    }
                }
                break;
            }
        }

        /// <summary>
        /// Removes the specified key from all registered cache providers.
        /// </summary>
        /// <param name="key">The unique cache key to remove.</param>
        /// <returns>A task that represents the asynchronous removal operation.</returns>
        /// <exception cref="ArgumentException">Thrown when the key is null or empty.</exception>
        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Cache key cannot be null or empty.", nameof(key));
            }

            _logger.LogDebug("Removing key '{Key}' from all cache providers", key);

            foreach (var provider in _providers)
            {
                await provider.RemoveAsync(key);
            }
        }

        #region

        private async Task<T> HandleCacheAside<T>(string key, Func<Task<T>> dataLoader, TimeSpan? timeToLive)
        {
            if (dataLoader == null)
            {
                _logger?.LogWarning("Cache miss for key '{Key}', and no fallback provided.", key);

                throw new InvalidOperationException("CacheAside strategy requires a dataLoader fallback.");
            }

            var keyLock = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
            await keyLock.WaitAsync();

            try
            {
                // Check again after acquiring the lock, maybe something else loaded it.
                var orderedProviders = OrderedCacheProviders();
                foreach (var provider in orderedProviders)
                {
                    var result = await provider.GetAsync<T>(key);
                    if (result != null)
                    {
                        _logger.LogDebug("Cache hit in provider {Provider} for key '{Key}'", provider.GetType().Name, key);

                        if (_promoteOnHit)
                        {
                            await PromoteToEarlierCachesAsync(key, result, provider, orderedProviders);
                        }

                        return result;
                    }
                }

                // If we reach here, it means the data was not found in any cache provider.
                var value = await dataLoader();

                foreach (var provider in _providers)
                {
                    await provider.SetAsync(key, value, timeToLive);
                }

                if (_promoteOnHit)
                {
                    await PromoteToEarlierCachesAsync(key, value, null, orderedProviders);
                }

                return value;
            }
            finally
            {
                keyLock.Release();
                _keyLocks.TryRemove(key, out _);
            }
        }

        private async Task<T> HandleReadThrough<T>(string key)
        {
            var orderedProviders = OrderedCacheProviders();

            foreach (var provider in orderedProviders)
            {
                var readThroughProvider = provider as IReadThroughCacheProvider;
                if (readThroughProvider != null)
                {
                    _logger.LogDebug("Using ReadThrough provider {Provider} for key '{Key}'", provider.GetType().Name, key);

                    var result = await readThroughProvider.GetOrLoadAsync<T>(key);
                    if (result != null)
                    {
                        if (_promoteOnHit)
                        {
                            await PromoteToEarlierCachesAsync(key, result, provider, orderedProviders);
                        }

                        return result;
                    }
                }
                
            }
            _logger.LogWarning("No ReadThrough provider resolved key '{Key}'; falling back to CacheAside with fallback if provided", key);

            // in cases the handle -through does not yield to result, the default behavior is to fall back to cache-aside.
            return await HandleCacheAside<T>(key, null, null);
        }

        private ICacheProvider[] OrderedCacheProviders()
            => _providers.OrderBy(p => p.LookupPriority).ToArray();

        private async Task PromoteToEarlierCachesAsync<T>(string key, T value, ICacheProvider hitProvider, ICacheProvider[] orderedProviders)
        {
            _logger.LogInformation("Promoting key '{Key}' from {FromProvider} to higher-tier caches", key, hitProvider.GetType().Name);

            for (var index = 0; index < orderedProviders.Length; index++)
            {
                var provider = orderedProviders[index];
                if (provider == hitProvider)
                {
                    break;
                }

                await provider.SetAsync(key, value);
            }
        }

        #endregion
    }
}
