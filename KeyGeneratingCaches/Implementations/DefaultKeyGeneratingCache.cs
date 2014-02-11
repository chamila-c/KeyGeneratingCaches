using System;
using KeyGeneratingCaches.Api;

namespace KeyGeneratingCaches.Implementations
{
    /// <summary>
    /// An implementation of IKeyGeneratingCache with full compliance to the API, but no actual underlying caching.
    /// Suitable for use as a default when no other implementation is provided.
    /// </summary>
    public class DefaultKeyGeneratingCache : IKeyGeneratingCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Implementations.DefaultKeyGeneratingCache"/> class.
        /// </summary>
        public DefaultKeyGeneratingCache ()
        {
        }



        public string Add<T>(T data)
        {
            // There's no caching going on, but we still need
            // to ensure that a new unique key is returned
            return Guid.NewGuid ().ToString ();
        }

        public CacheEntry<T> Get<T>(string key, Func<T> cacheMissHandler)
        {
            // Need to always execute the cache miss handler
            // (since there's no actual caching going on!)...
            var dataFromSource = cacheMissHandler ();

            // ...and then pretend we're adding it to the 'cache',
            // which most importantly, ensures a new key is generated
            key = Add (dataFromSource);

            return new CacheEntry<T>
            {
                Key = key,
                Data = dataFromSource
            };
        }

        public void Remove(string key)
        {
            // Not implemented
        }
    }
}

