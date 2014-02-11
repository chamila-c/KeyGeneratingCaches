using System;

namespace KeyGeneratingCaches.Api.Verification
{
    /// <summary>
    /// A test helper that can be used to verify that an implementation of IKeyGeneratingCache conforms to the public API
    /// </summary>
    public class ApiTester
    {
        private readonly IKeyGeneratingCache _testSubject;


        /// <summary>
        /// Initializes a new instance of the <see cref="KeyGeneratingCaches.Api.Verification.ApiTester"/> class.
        /// </summary>
        /// <param name="testSubject">The implementation of IKeyGeneratingCache that is to be tested</param>
        public ApiTester (IKeyGeneratingCache testSubject)
        {
            _testSubject = testSubject;
        }



        public bool PassesAllApiTests()
        {
            return AValidCacheKeyIsReturnedOnAdd ()
            && TheCacheMissHandlerIsExecutedForANonExistentKey ()
            && AValidCacheKeyIsReturnedFromGetForANonExistentKey ()
            && AValidCacheKeyIsReturnedFromGetForAGenuineKey ()
            && ANewCacheKeyIsReturnedForACacheMiss ()
            && RemoveDoesNotThrowAnyExceptions ()
            && TheExpectedDataIsReturnedFromGetForANonExistentKey ()
            && NullKeySuppliedToGetDoesNotResultInExceptions ()
            && EmptyKeySuppliedToGetDoesNotResultInExceptions ();
        }

        public bool AValidCacheKeyIsReturnedOnAdd()
        {
            try
            {
                var resultOfAdd = _testSubject.Add (new System.FileStyleUriParser ());
                return !String.IsNullOrWhiteSpace (resultOfAdd);
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool TheCacheMissHandlerIsExecutedForANonExistentKey()
        {
            var nonExistentKey = Guid.NewGuid ().ToString ();
            var result = false;
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                result = true;
                return new FileStyleUriParser ();
            };
            _testSubject.Get (nonExistentKey, cacheMissHandler);
            return result;
        }

        public bool AValidCacheKeyIsReturnedFromGetForANonExistentKey()
        {
            var nonExistentKey = Guid.NewGuid ().ToString ();
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return new FileStyleUriParser ();
            };
            var cacheMissResult = _testSubject.Get (nonExistentKey, cacheMissHandler);
            return cacheMissResult != null && !String.IsNullOrWhiteSpace (cacheMissResult.Key);
        }

        public bool AValidCacheKeyIsReturnedFromGetForAGenuineKey()
        {
            var dataToCache = new FileStyleUriParser ();
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return new FileStyleUriParser ();
            };

            var genuineKey = _testSubject.Add (dataToCache);
            var cachedData = _testSubject.Get (genuineKey, cacheMissHandler);

            return cachedData != null && !String.IsNullOrWhiteSpace (cachedData.Key);
        }

        public bool ANewCacheKeyIsReturnedForACacheMiss()
        {
            var nonExistentKey = Guid.NewGuid ().ToString ();
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return new FileStyleUriParser ();
            };
            var cacheMissResult = _testSubject.Get (nonExistentKey, cacheMissHandler);
            return cacheMissResult != null && !nonExistentKey.Equals (cacheMissResult.Key);
        }

        public bool RemoveDoesNotThrowAnyExceptions()
        {
            var madeUpKeyForRemoval = Guid.NewGuid ().ToString ();
            try
            {
                _testSubject.Remove(madeUpKeyForRemoval);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool TheExpectedDataIsReturnedFromGetForANonExistentKey()
        {
            var nonExistentKey = Guid.NewGuid ().ToString ();
            var expectedData = new FileStyleUriParser ();
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return expectedData;
            };
            var getResult = _testSubject.Get (nonExistentKey, cacheMissHandler);
            return getResult.Data.Equals(expectedData);
        }

        public bool NullKeySuppliedToGetDoesNotResultInExceptions()
        {
            string nullKey = null;
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return new FileStyleUriParser ();
            };

            try
            {
                _testSubject.Get (nullKey, cacheMissHandler);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EmptyKeySuppliedToGetDoesNotResultInExceptions()
        {
            var emptyKey = String.Empty;
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return new FileStyleUriParser ();
            };

            try
            {
                _testSubject.Get (emptyKey, cacheMissHandler);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
