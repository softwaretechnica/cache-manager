using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ChocolateSmoke.Tests
{
    public class PassThroughTests
    {
        [Fact]
        public async Task PassThrough_UsesFallbackOnly()
        {
            var manager = new CacheManager(NullLogger<CacheManager>.Instance)
                .WithStrategy(CacheStrategy.PassThrough);

            var result = await manager.GetAsync("key", () => Task.FromResult("from-db"));
            Assert.Equal("from-db", result);
        }
    }
}