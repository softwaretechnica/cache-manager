using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using Xunit;

using ChocolateSmoke.Tests.Core;

namespace ChocolateSmoke.Tests
{
    public class CacheAsideTests
    {
        [Fact]
        public async Task SingleProvider_CacheAside_MissThenHit()
        {
            var provider = new MemoryCacheProvider();
            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.CacheAside)
                .UseProvider(provider);

            var value = await manager.GetAsync<string>("key", () => Task.FromResult("loaded"));
            Assert.Equal("loaded", value);

            var cached = await provider.GetAsync<string>("key");
            Assert.Equal("loaded", cached);
        }

        [Fact]
        public async Task MultiProvider_CacheAside_PromotesOnHit()
        {
            var redis = new RedisCacheProvider();
            var memory = new MemoryCacheProvider();

            var key = "key";
            var value = "value";
            var expectedValue = "value";

            await redis.SetAsync(key, value); 

            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.CacheAside)
                .WithPromotionOnHit(true)
                .UseProvider(memory)
                .UseProvider(redis);

            var result = await manager.GetAsync<string>(key, () => Task.FromResult(value));
            Assert.Equal(expectedValue, result);

            var promoted = await memory.GetAsync<string>(key);
            Assert.Equal(expectedValue, promoted);
        }

        [Fact]
        public async Task MultiProvider_CacheAside_Off_PromotesOnHit()
        {
            var redis = new RedisCacheProvider();
            var memory = new MemoryCacheProvider();

            var key = "key";
            var value = "value";
            var expectedValue = "value";

            await redis.SetAsync(key, value);

            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.CacheAside)
                .WithPromotionOnHit(true)
                .UseProvider(memory)
                .UseProvider(redis);

            var result = await manager
                .WithPromotionOnHit(false)                
                .GetAsync(key, () => Task.FromResult(value));
            
            Assert.Equal(expectedValue, result);

            var promoted = await memory.GetAsync<string>(key); 
            Assert.Null(promoted);
        }
    }
}
