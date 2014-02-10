using System;
using KeyGeneratingCaches.Api;

namespace Implementations
{
    public class NotImplementedKeyGeneratingCache : IKeyGeneratingCache
    {
        public NotImplementedKeyGeneratingCache ()
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

