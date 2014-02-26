using System;
using KeyGeneratingCaches.Api.Verification;
using KeyGeneratingCaches.Implementations;
using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Collections.Specialized;

namespace KeyGeneratingCaches.UnitTests.Implementations
{
    [TestFixture ()]
    public class LockingKeyGeneratingCacheTests
    {
        private readonly ApiTester _apiTester;



        public LockingKeyGeneratingCacheTests ()
        {
            // TODO: Use the constructor overload that takes an `ObjectCache` instance
            // TODO: and supply a mocked `ObjectCache`
            // For now, using the default constructor, which will spin up a `MemoryCache`
            _apiTester = new ApiTester(new LockingKeyGeneratingCache());
        }



        [Test()]
        /// <summary>
        /// Tests API conformance using the <see cref="KeyGeneratingCaches.Api.Verification.ApiTester"/>
        /// class' 'PassesAllApiTests' method
        /// </summary>
        public void PassesAllApiTest()
        {
            Assert.IsTrue (_apiTester.PassesAllApiTests (this.GetType().FullName +  ".PassesAllApiTests"));
        }



        // TODO: Thread safety tests

        // TODO: Synchronisation (b/locking) tests



        // Based on this test case's output, the MemoryCache appears not to be
        // ejecting anything, as memory consumption simply grows until the test
        // ends. Need to investigate further.
        [Category("Excluded")]
        [Test()]
        public void MemoryProfiling()
        {
            var cacheSettings = new NameValueCollection ();
            cacheSettings.Add ("cacheMemoryLimitMegabytes", "200");
            cacheSettings.Add ("pollingInterval", "00:00:02");
            var memoryCache = new MemoryCache("Test.Cache", cacheSettings);

            Console.WriteLine (memoryCache.CacheMemoryLimit);
            Console.WriteLine (memoryCache.PhysicalMemoryLimit);
            Console.WriteLine (memoryCache.PollingInterval);

            var objectUnderTest = new LockingKeyGeneratingCache (memoryCache);
            Console.WriteLine (Process.GetCurrentProcess ().WorkingSet64);
            for (var outerLoop = 0; outerLoop < 20; outerLoop++)
            {
                for (var innerLoop = 0; innerLoop < 100000; innerLoop++)
                {
                    objectUnderTest.Add (new FileStyleUriParser());
                }
                Console.WriteLine (Process.GetCurrentProcess ().WorkingSet64);
            }
        }
    }
}

