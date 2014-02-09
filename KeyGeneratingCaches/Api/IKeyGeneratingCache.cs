using System;

namespace KeyGeneratingCaches.Api
{
    /// <summary>
    /// A caching API that ensures a new unique cache key is generated whenever:
    /// - an item is added;
    /// - a cache miss occurs
    /// </summary>
    public interface IKeyGeneratingCache
    {
        /// <summary>
        /// Add an item to the cache
        /// </summary>
        /// <param name="data">The item to be added</param>
        /// <typeparam name="T">The type of the item being added</typeparam>
        /// <returns>The unique cache key to be used when next accessing the cached item</returns>
        string Add<T>(T data);


        /// <summary>
        /// Retrieve an item from the cache, or if unavailable in cache,
        /// from the underlying data source
        /// </summary>
        /// <param name="key">The cache key for the item to be retrieved</param>
        /// <param name="cacheMissHandler">The function to be executed (to retrieve the item from the underlying data source) if a cache miss occurs for the requested key</param>
        /// <typeparam name="T">The type of the item to be retrieved</typeparam>
        /// <returns>A CacheEntry that contains:
        ///  - the unique cache key to be used when next accessing the cached item;
        ///  - the retrieved item</returns>
        CacheEntry<T> Get<T>(string key, Func<T> cacheMissHandler);


        /// <summary>
        /// Remove (eject) an item from the cache.
        /// 
        /// This method is non-binding, and the underlying
        /// cache implementation is free to do any of the
        /// following when this method is called:
        /// - immediately eject the item;
        /// - defer ejection to a later point in time;
        /// - do nothing
        /// 
        /// This method can be called as a hint to the
        /// underlying cache implementation that resources
        /// can be recovered by ejecting the specified item
        /// 
        /// Calling code should not rely on cache misses
        /// occurring immediately after this method is called
        /// </summary>
        /// <param name="key">The cache key for the item that is to be removed</param>
        void Remove(string key);
    }
}

