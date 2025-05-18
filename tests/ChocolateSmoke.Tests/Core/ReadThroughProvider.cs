using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChocolateSmoke.Tests.Core
{
    public class ReadThroughProvider : IReadThroughCacheProvider
    {
        private readonly Dictionary<string, object> _store = new();
        public int LookupPriority { get; set; }

        public Task<T> GetAsync<T>(string key) => Task.FromResult(default(T));
        public Task RemoveAsync(string key) => Task.CompletedTask;
        public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task<T> GetOrLoadAsync<T>(string key)
        {
            return Task.FromResult((T)(object)"loaded");
        }
    }
}

