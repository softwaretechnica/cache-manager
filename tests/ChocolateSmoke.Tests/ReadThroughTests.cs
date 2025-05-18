using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using ChocolateSmoke.Tests.Core;

namespace ChocolateSmoke.Tests
{
    public class ReadThroughTests
    {     
        [Fact]
        public async Task ReadThrough_ProviderLoadsOnMiss()
        {
            var provider = new ReadThroughProvider();
            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.ReadThrough)
                .UseProvider(provider);

            var result = await manager.GetAsync<string>("load-me");
            Assert.Equal("loaded", result);
        }
    }
}