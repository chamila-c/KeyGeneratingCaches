using System;
using NUnit.Framework;
using KeyGeneratingCaches.Api.Verification;
using KeyGeneratingCaches.Implementations;

namespace KeyGeneratingCaches.UnitTests.Implementations
{
    [TestFixture ()]
    public class NonLockingKeyGeneratingCacheTests
    {
        private readonly ApiTester _apiTester;



        public NonLockingKeyGeneratingCacheTests()
        {
            // TODO: Use the constructor overload that takes an `ObjectCache` instance
            // TODO: and supply a mocked `ObjectCache`
            // For now, using the default constructor, which will spin up a `MemoryCache`
            _apiTester = new ApiTester(new NonLockingKeyGeneratingCache());
        }



        [Test()]
        /// <summary>
        /// Tests API conformance using the KeyGeneratingCaches.Api.Verification.ApiTester
        /// class' 'PassesAllApiTests' method
        /// </summary>
        public void PassesAllApiTest()
        {
            Assert.IsTrue (_apiTester.PassesAllApiTests (this.GetType().FullName +  ".PassesAllApiTests"));
        }
    }
}

