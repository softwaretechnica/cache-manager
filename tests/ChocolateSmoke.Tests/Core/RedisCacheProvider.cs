using System.Collections.Generic;

namespace ChocolateSmoke.Tests.Core;

public class RedisCacheProvider : UnitTestMemoryProviderBase
{
    public RedisCacheProvider(int lookupPriority = 1, IDictionary<string, object> preInitializedCache = default) : base(lookupPriority)
    {

    }
}