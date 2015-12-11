using System;
using System.Runtime.Caching;
using System.Threading;
using KeyGeneratingCaches.Api;

namespace KeyGeneratingCaches.Implementations
{
    /// <summary>
    /// An implementation of IKeyGeneratingCache that optimises for concurrency
    /// (by avoiding any locking or synchronisation) at the expense of potentially
    /// higher cache miss rates.
    /// If multiple concurrent threads request an item that triggers a cache miss,
    /// each thread will independently fetch the data from the underlying source. 
    /// </summary>
    public class NonLockingKeyGeneratingCache : IKeyGeneratingCache
    {
        // Private class for boxing the actual data before
        // dropping it into the cache so there's no abiguity
        // around nulls
        private class DataBox<T>
        {
            public T Data { get; set; }
        }


        // The underlying cache
        private readonly ObjectCache _objectCache;



        /// <summary>
        /// Initializes a new instance of the <see cref="NonLockingKeyGeneratingCache"/> class
        /// that utilises a <see cref="System.Runtime.Caching.MemoryCache"/> as the underlying cache
        /// </summary>
        public NonLockingKeyGeneratingCache ()
        {
            _objectCache = new MemoryCache (this.GetType().FullName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonLockingKeyGeneratingCache"/> class
        /// that utilises the supplied <see cref="System.Runtime.Caching.ObjectCache"/> as the underlying cache 
        /// </summary>
        /// <param name="objectCache">A <see cref="System.Runtime.Caching.ObjectCache"/> to be used as the underlying cache</param>
        public NonLockingKeyGeneratingCache(ObjectCache objectCache)
        {
            _objectCache = objectCache;
        }



        public string Add<T>(T data)
        {
            // All additions are done on a new, unique, key
            var key = GenerateNewKey ();
            var dataBox = new DataBox<T>
            {
                Data = data
            };

            // Defaults for `CacheItemPolicy` should be fine
            _objectCache.Add (new CacheItem (key, dataBox), new CacheItemPolicy ());

            return key;
        }

        public CacheEntry<T> Get<T>(string key, Func<T> cacheMissHandler)
        {
            // Avoid getting tripped up by invalid keys
            if (String.IsNullOrWhiteSpace (key))
            {
                key = GenerateNewKey ();
            }

            // Try to get the cache item from the cache
            var cacheItem = _objectCache.Get (key);

            // Check if it was a cache miss
            // (or if we got a different type back from the cache!)
            if (cacheItem == null
                || !(cacheItem is DataBox<T>))
            {
                // No luck, it's a miss, so need to do the hard work
                var dataFromSource = cacheMissHandler ();

                // Cache it
                key = Add (dataFromSource);

                // And fill out the 'cache' item
                cacheItem = new DataBox<T>
                {
                    Data = dataFromSource
                };
            }

            // Since the cacheItem is an `object`  we need to make sure it's
            // cast correctly first - by this stage it should be of the correct
            // type, so a direct cast is safe
            var dataBox = (DataBox<T>)cacheItem;

            return new CacheEntry<T>{ Key = key, Data = dataBox.Data };
        }

        public void Remove(string key)
        {
            // Because 'Remove' is non-binding, we try
            // our best to remove from the underlying
            // object cache, but swallow any errors
            try
            {
                _objectCache.Remove (key);
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

