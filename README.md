# CacheManager

A fluent, strategy-driven caching manager for .NET supporting multiple cache providers, cache tiering, and promotion. Implements Chain of Responsibility and Strategy patterns for flexible, testable cache orchestration.

---

## 🚀 Features

- 🧱 Chainable cache provider configuration (e.g., MemoryCache + Redis)
- 🎯 Strategy-based behavior: CacheAside, WriteThrough, ReadThrough, WriteAround, PassThrough
- 🔄 Optional promotion on cache hits
- 🧵 Thread-safe access with per-key locking
- ✅ Fully unit-tested with both fake and mock providers

---

## 🧠 Design Patterns Used

- **Fluent Interface**
- **Strategy Pattern**
- **Chain of Responsibility**
- **Template Method**
- **Decorator-like cache layering**

---

## 📦 Installation

This is a library project. Clone the repository and add the project reference to your solution:

```bash
git clone https://github.com/your-user/CacheManager.git
```

---

## 🧪 Running Tests

Tests are written using xUnit and run against real in-memory providers.

```bash
dotnet test
```

---

## 📝 Usage Example

```csharp
var manager = new CacheManager(logger)
    .UseProvider(new MemoryCacheProvider())
    .UseProvider(new RedisCacheProvider())
    .WithStrategy(CacheStrategy.CacheAside)
    .WithPromotionOnHit(true);

var result = await manager.GetAsync("product:123", async () => await LoadFromDb());
```

---

## 📂 Project Structure

```
/src        --> Main library project (ChocolateSmoke)
/tests      --> Unit tests (ChocolateSmoke.Tests)
/docs       --> Project documentation (optional)
```

## 📄 License

MIT © SoftwareTechinca. See [LICENSE](LICENSE) for details.