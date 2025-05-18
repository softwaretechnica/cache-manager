using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChocolateSmoke.Tests.Core;

public class UnitTestMemoryProviderBase : ICacheProvider
{
    private readonly IDictionary<string, object> _cache;

    public UnitTestMemoryProviderBase(int lookupPriority, IDictionary<string, object> preInitializedCache = default)
    {
        if (preInitializedCache != default)
        {
            _cache = new Dictionary<string, object>(preInitializedCache);
        }
        else
        {
            _cache = new Dictionary<string, object>();
        }
        LookupPriority = lookupPriority;
    }

    public Task<T> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            return Task.FromResult((T)item);
        }

        return Task.FromResult<T>(default);

    }

    public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        _cache[key] = value;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key, out _);
        return Task.CompletedTask;

    }

    public int LookupPriority { get; }
}