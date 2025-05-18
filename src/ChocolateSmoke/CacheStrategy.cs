namespace ChocolateSmoke
{
    /// <summary>
    /// Represents the different caching strategies that can be applied within a cache management system.
    /// These strategies determine how data is read from and written to the cache in relation to the underlying data source.
    /// </summary>
    public enum CacheStrategy
    {
        /// <summary>
        /// Cache-Aside: The application checks the cache before querying the data source. On a cache miss, it loads the data from the source and stores it in the cache.
        /// </summary>
        CacheAside = 0,

        /// <summary>
        /// Write-Through: Data is written to the cache and immediately persisted to the underlying data store.
        /// Ensures consistency but may introduce write latency.
        /// </summary>
        WriteThrough = 1,

        /// <summary>
        /// Read-Through: The cache itself is responsible for loading data from the source if the requested item is not already cached.
        /// Simplifies the application logic.
        /// </summary>
        ReadThrough = 2,

        /// <summary>
        /// Write-Around: Data is written directly to the data source, bypassing the cache.
        /// Useful when cached data is rarely read after being written.
        /// </summary>
        WriteAround = 3,

        /// <summary>
        /// Pass-Through: All operations bypass the cache and go directly to the data source.
        /// Typically used in scenarios where caching is temporarily disabled.
        /// </summary>
        PassThrough = 4
    }
}