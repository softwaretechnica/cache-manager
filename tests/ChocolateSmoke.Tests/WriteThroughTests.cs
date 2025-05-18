using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using ChocolateSmoke.Tests.Core;

namespace ChocolateSmoke.Tests
{
    public class WriteThroughTests
    {
        [Fact]
        public async Task WriteThrough_WritesToStoreAndCache()
        {
            var provider = new MemoryCacheProvider();
            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.WriteThrough)
                .UseProvider(provider);

            var store = new Dictionary<string, string>();
            await manager.SetAsync("k", "v", null, v => {
                store["k"] = v;
                return Task.CompletedTask;
            });

            Assert.Equal("v", store["k"]);
            Assert.Equal("v", await provider.GetAsync<string>("k"));
        }
    }
}