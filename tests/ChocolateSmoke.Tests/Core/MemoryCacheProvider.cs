using System.Collections.Generic;

namespace ChocolateSmoke.Tests.Core;

public class MemoryCacheProvider : UnitTestMemoryProviderBase
{
    public MemoryCacheProvider(int lookupPriority = 0, IDictionary<string, object> preInitializedCache = default) : base(lookupPriority)
    {

    }
}