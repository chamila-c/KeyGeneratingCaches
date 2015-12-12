KeyGeneratingCaches
===================

[![Build Status](https://travis-ci.org/chamila-c/KeyGeneratingCaches.svg)](https://travis-ci.org/chamila-c/KeyGeneratingCaches)

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

Using a KeyGeneratingCache in an eponymous 'book repository':


    public interface IBookRepository
    {
        Book GetBook (string isbn);
        void AddBook (Book book);
        Book UpdateBook (string isbn, Book newBookData);
    }


    public class Book
    {
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
    }


    public class BookRepository
    {
        private IKeyGeneratingCache _cache;
        private string _dataFile;
        private ConcurrentDictionary<string,string> _cacheKeyMap = new ConcurrentDictionary<string,string>();

        public BookRepository (string dataFile, IKeyGeneratingCache cache)
        {
            _dataFile = dataFile;
            _cache = cache;
        }

        public Book GetBook(string isbn)
        {
            string cacheKey;

            // TryGetValue will return null if there isn't an existing entry...
            _cacheKeyMap.TryGetValue (isbn, out cacheKey);

            // ... but it's safe to call Get even with a null key, as the
            // cache will simply issue a new one
            var cacheEntry = _cache.Get (cacheKey, () => { /* Do stuff to fetch the book if the cache misses */ });

            // Re/associate the isbn with the returned key, which may be a
            // new key, or the existing one - doesn't matter either way
            _cacheKeyMap [isbn] = cacheEntry.Key;

            return cacheEntry.Data;
        }

        public void AddBook(Book book)
        {
            /* Do stuff to add the book details to somewhere */

            // Now also add it to the cache...
            var cacheKey = _cache.Add(book);

            /// ...and the cache key map.
            // Since cacheKey is going to be unique (all `Add`s return a
            // new unique key) it's safe to add directly
            _cacheKeyMap[book.ISBN] = cacheKey;

        }

        public Book UpdateBook (string isbn, Book newBookData)
        {
            var updatedBook = new Book {
                ISBN = isbn,
                Name = newBookData.Name,
                Author = newBookData.Author
            };

            /* Do stuff to update the book details to somewhere */

            // Add the updated record to the cache...
            AddBook (updatedBook);

            // .. and don't worry about the old entry. Once the cache
            // key map is updated, it will no longer be fetched from
            // the cache, and will therefore get aged out of the cache

            return updatedBook;
        }
    }
