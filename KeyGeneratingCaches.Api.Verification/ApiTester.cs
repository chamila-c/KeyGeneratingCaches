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



        public bool PassesAllApiTests(string testCaseName)
        {
            testCaseName = testCaseName ?? "Passes All Api Tests";

            var test1 = AValidCacheKeyIsReturnedOnAdd ();
            var test2 = TheCacheMissHandlerIsExecutedForANonExistentKey ();
            var test3 = AValidCacheKeyIsReturnedFromGetForANonExistentKey ();
            var test4 = AValidCacheKeyIsReturnedFromGetForAGenuineKey ();
            var test5 = ANewCacheKeyIsReturnedForACacheMiss ();
            var test6 = RemoveDoesNotThrowAnyExceptions ();
            var test7 = TheExpectedDataIsReturnedFromGetForANonExistentKey ();
            var test8 = TheExpectedDataIsReturnedFromGetForAGenuineKey ();
            var test9 = NullKeySuppliedToGetDoesNotResultInExceptions ();
            var test10 = EmptyKeySuppliedToGetDoesNotResultInExceptions ();


            Console.WriteLine ();
            Console.WriteLine (String.Format("Running all IKeyGeneratingCache Api Tests for test case '{0}'...", testCaseName));

            Console.WriteLine (FormatForOutput("A valid cache key is returned on `Add`", test1));
            Console.WriteLine (FormatForOutput("The cache miss handler is executed for a non-existent key", test2));
            Console.WriteLine (FormatForOutput("A valid cache key is returned from `Get` for a non-existent key", test3));
            Console.WriteLine (FormatForOutput("A valid cache key is returned from `Get` for a genuine key", test4));
            Console.WriteLine (FormatForOutput("A new cache key is returned for a cache miss", test5));
            Console.WriteLine (FormatForOutput("`Remove` does not throw any exceptions", test6));
            Console.WriteLine (FormatForOutput("The expected data is returned from `Get` for a non-existent key", test7));
            Console.WriteLine (FormatForOutput("The expected data is returned from `Get` for a genuine key", test8));
            Console.WriteLine (FormatForOutput("Null key supplied to `Get` does not result in exceptions", test9));
            Console.WriteLine (FormatForOutput("Empty key supplied to `Get` does not result in exceptions", test10));

            Console.WriteLine ("...test run complete");
            Console.WriteLine ();


            return test1
                && test2
                && test3
                && test4
                && test5
                && test6
                && test7
                && test8
                && test9
                && test10;
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

        public bool TheExpectedDataIsReturnedFromGetForAGenuineKey()
        {
            var expectedData = new FileStyleUriParser ();
            Func<FileStyleUriParser> cacheMissHandler = () =>
            {
                return expectedData;
            };
            var genuineKey = _testSubject.Add (expectedData);
            var getResult = _testSubject.Get (genuineKey, cacheMissHandler);
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



        private string FormatForOutput(string testDescription, bool testPassed)
        {
            var resultString = testPassed.ToString ();
            var indentation = "\t";
            if (!testPassed)
            {
                resultString = testPassed.ToString ().ToUpperInvariant () + "\t!!!!";
                indentation = "!!!!\t";
            }
            return indentation + testDescription + ": " + resultString;
        }
    }
}
