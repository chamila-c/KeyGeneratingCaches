using System;

namespace KeyGeneratingCaches.Api
{
    /// <summary>
    /// A data class used for returning the combination of:
    ///  - an item retrieved from cache (or underlying source);
    ///  - the unique cache key to be used for next accessing the retrived item
    /// </summary>
    public class CacheEntry<T>
    {
        /// <summary>
        /// The unique cache key to be used for next accessing the cached item
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The item retrieved from cache (or underlying source)
        /// </summary>
        public T Data { get; set; }
    }
}

