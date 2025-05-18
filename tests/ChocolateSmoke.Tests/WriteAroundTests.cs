using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using ChocolateSmoke.Tests.Core;

namespace ChocolateSmoke.Tests
{
    public class WriteAroundTests
    {
        [Fact]
        public async Task WriteAround_OnlyWritesToStore()
        {
            var provider = new MemoryCacheProvider();
            var store = new Dictionary<string, string>();

            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.WriteAround)
                .UseProvider(provider);

            await manager.SetAsync("key", "val", null, v =>
            {
                store["key"] = v;
                return Task.CompletedTask;
            });

            Assert.Equal("val", store["key"]);
            Assert.Null(await provider.GetAsync<string>("key"));
        }
    }
}