KeyGeneratingCaches
===================

Caches inspired by the key-based expiration method described here: 

[How key-based cache expiration works](http://signalvnoise.com/posts/3113-how-key-based-cache-expiration-works)

In this case, however, instead of tying the cache key to the entity, the cache key is simply a unique key that cycles whenever changes are made, or when cache misses occur.


API
---

The KeyGeneratingCache API requires that a new unique cache key is generated whenever:

 - an item is added
 - a cache miss occurs


Recommended For
---------------

Ideal for repositories that fully manage a data source, i.e. where all updates happen through the repository.
A KeyGeneratingCache can be plugged into the repository to optimise performance for reads.


Example
--------

    // Coming soon...
