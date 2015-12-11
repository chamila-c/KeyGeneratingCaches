using System;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Threading;
using KeyGeneratingCaches.Api;

namespace KeyGeneratingCaches.Implementations
{
    /// <summary>
    /// An implementation of IKeyGeneratingCache that attempts to balance both
    /// high concurrency and low cache miss rates through the use of optimised
    /// locking and synchronisation.
    /// If multiple concurrent threads request an item that triggers a cache miss,
    /// this implementation will attempt to (as much as possible) ensure that only the
    /// first thread fetches the data from the underlying source, with the remaining
    /// threads blocking until the data is loaded into the cache by the first thread.
    /// N.B. If the data retrieved by the first thread is not held by the cache
    /// (e.g. cache full) then the later threads will still fetch from source.
    /// </summary>
    public class LockingKeyGeneratingCache : IKeyGeneratingCache
    {
        // Private class for boxing the actual data before
        // dropping it into the cache, and, importantly,
        // info needed for redirecting cache lookups
        private class DataBox<T>
        {
            public T Data { get; set; }
            public bool IsRedirect { get; set; }
            public string RedirectKey { get; set; }
        }



        // Lock management
        private static readonly object _fallbackLock = new object();
        private static readonly ConcurrentDictionary<string, object> _lockCollection = new ConcurrentDictionary<string, object> ();

        // The underlying cache
        private readonly ObjectCache _objectCache;



        /// <summary>
        /// Initializes a new instance of the <see cref="LockingKeyGeneratingCache"/> class
        /// that utilises a <see cref="System.Runtime.Caching.MemoryCache"/> as the underlying cache
        /// </summary>
        public LockingKeyGeneratingCache()
        {
            _objectCache = new MemoryCache (this.GetType().FullName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LockingKeyGeneratingCache"/> class
        /// that utilises the supplied <see cref="System.Runtime.Caching.ObjectCache"/> as the underlying cache 
        /// </summary>
        /// <param name="objectCache">A <see cref="System.Runtime.Caching.ObjectCache"/> to be used as the underlying cache</param>
        public LockingKeyGeneratingCache(ObjectCache objectCache)
        {
            _objectCache = objectCache;
        }



        public string Add<T>(T data)
        {
            // All additions are done on a new, unique, key
            var key = GenerateNewKey ();
            var dataBox = new DataBox<T>
            {
                Data = data,
                IsRedirect = false,
                RedirectKey = String.Empty
            };
            _objectCache.Add (new CacheItem (key, dataBox), new CacheItemPolicy ());

            // To support granular locking for cache misses,
            // we also add a new 'lock' object for each new key
            _lockCollection [key] = new object ();

            // Adding key to 'lock collection' _must_ be complete before
            // key is returned, to prevent potential races with 'Get'
            Thread.MemoryBarrier ();

            return key;
        }

        public CacheEntry<T> Get<T>(string key, Func<T> cacheMissHandler)
        {
            // Avoid getting tripped up by invalid keys by assigning
            // a new key for null or empty keys
            var originalKey = key;
            if (String.IsNullOrWhiteSpace (originalKey))
            {
                originalKey = GenerateNewKey ();
            }

            // For now, assume the key won't change, so 'new' = 'old'
            var newKey = originalKey;

            // Try to get the cache item from the cache
            var cacheItem = _objectCache.Get (originalKey);
            Thread.MemoryBarrier();     // Ensures cache 'get' is current/'fresh' before testing it

            // Check if it was a cache miss
            // (or if we got a different type back from the cache)
            if (cacheItem == null
                || !(cacheItem is DataBox<T>))
            {
                // Cache miss, so now need to enter a (double-checked) lock, in case
                // multiple threads have all missed at the same time. The lock will
                // ensure that only one thread does the hard work, while the others
                // simply block and wait, thereby preserving resources that would
                // otherwise be consumed by multiple threads trying to get the data
                // from the underlying source.
                // The blocked thread(s) will need to access the cached data via a
                // 'redirect' entry, since their key will be outdated once the
                // first thread has retrieved the data from source


                // Since locking should be as granular as possible (to prevent redundant
                // blocking of threads) the locking is done on a per-key basis, so we
                // need to get the lock object corresponding to the key
                object lockObject;
                var getLockObject = _lockCollection.TryGetValue(originalKey, out lockObject);

                // No need for `MemoryBarrier` here, as the `TryGetValue` will have already
                // applied appropriate memory fencing

                // If no lock object exists (e.g. null key, or old key),
                // create one, and add it (safely) to the 'lock collection'
                if (!getLockObject || lockObject == null)
                {
                    lock (_fallbackLock)
                    {
                        // Re-test in case another thread already added it...
                        getLockObject = _lockCollection.TryGetValue(originalKey, out lockObject);
                        if (!getLockObject || lockObject == null)
                        {
                            lockObject = new object ();
                            _lockCollection [originalKey] = lockObject;
                        }
                    }
                }

                lock (lockObject)
                {
                    // Check if it's still a cache miss (or different type), in case
                    // this thread was one of several that missed, and one of the
                    // others has already done the hard work
                    cacheItem = _objectCache.Get (originalKey);
                    if (cacheItem == null
                        || !(cacheItem is DataBox<T>))
                    {
                        // No luck, it's still a miss, so this thread needs to do the hard work
                        var dataFromSource = cacheMissHandler ();

                        // Add to cache, getting a new key
                        newKey = Add(dataFromSource);

                        // Fill out the 'cache' item
                        cacheItem = new DataBox<T>
                        {
                            Data = dataFromSource,
                            IsRedirect = false,
                            RedirectKey = String.Empty
                        };

                        // Also cache a redirect so that any threads that were blocked can get
                        // the cached value using the new key. Using a redirect instead of the
                        // actual data to ensure the correct (new) key is returned to the consumer
                        var redirect = new DataBox<T> {
                            IsRedirect = true,
                            RedirectKey = newKey,
                            Data = default(T)
                        };
                        _objectCache.Add (new CacheItem(originalKey, redirect), new CacheItemPolicy());

                        // Ensure redirect is always added _before_ removing old lock object
                        Thread.MemoryBarrier ();

                        // Get rid of the dictionary entry containing the lock object
                        // for the old key. This is to reclaim the memory, otherwise with
                        // the key generation strategy, memory usage would grow incessantly.
                        object removedLock;
                        _lockCollection.TryRemove (originalKey, out removedLock);
                    }
                }
            }

            // Since the cacheItem is an `object`  we need to make sure it's
            // cast correctly first - by this stage it should be of the correct
            // type, so a direct cast is safe
            var dataBox = (cacheItem as DataBox<T>);
            // If the cacheItem is a redirect, just return the data found at the new key
            if (dataBox.IsRedirect) 
            {
                return Get (dataBox.RedirectKey, cacheMissHandler);
            }

            return new CacheEntry<T>{ Key = newKey, Data = dataBox.Data };
        }

        public void Remove(string key)
        {
            // Because 'Remove' is non-binding, we try
            // our best to remove from the underlying
            // object cache, (and the dictionary entry
            // containing the lock object for the key)
            // but swallow any errors
            try
            {
                _objectCache.Remove (key);
                object removedLock;
                _lockCollection.TryRemove(key, out removedLock);
            }
            catch(Exception)
            {
            }
        }



        private string GenerateNewKey()
        {
            return Guid.NewGuid().ToString();
        }
    }
}

