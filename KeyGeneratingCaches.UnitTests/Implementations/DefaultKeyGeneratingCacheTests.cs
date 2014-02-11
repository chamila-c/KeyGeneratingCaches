using System;
using NUnit.Framework;
using KeyGeneratingCaches.Implementations;
using KeyGeneratingCaches.Api.Verification;

namespace KeyGeneratingCaches.UnitTests.Implementations
{
    [TestFixture ()]
    public class DefaultKeyGeneratingCacheTests
    {
        private readonly ApiTester _apiTester = new ApiTester(new DefaultKeyGeneratingCache());

        [Test()]
        public void ApiTestAValidCacheKeyIsReturnedOnAdd()
        {
            Assert.IsTrue (_apiTester.AValidCacheKeyIsReturnedOnAdd());
        }

        [Test()]
        public void ApiTestTheCacheMissHandlerIsExecutedForANonExistentKey()
        {
            Assert.IsTrue (_apiTester.TheCacheMissHandlerIsExecutedForANonExistentKey ());
        }

        [Test()]
        public void ApiTestAValidCacheKeyIsReturnedFromGetForANonExistentKey()
        {
            Assert.IsTrue (_apiTester.AValidCacheKeyIsReturnedFromGetForANonExistentKey ());
        }

        [Test()]
        public void ApiTestAValidCacheKeyIsReturnedFromGetForAGenuineKey()
        {
            Assert.IsTrue (_apiTester.AValidCacheKeyIsReturnedFromGetForAGenuineKey ());
        }

        [Test()]
        public void ApiTestANewCacheKeyIsReturnedForACacheMiss()
        {
            Assert.IsTrue (_apiTester.ANewCacheKeyIsReturnedForACacheMiss ());
        }

        [Test()]
        public void ApiTestRemoveDoesNotThrowAnyExceptions()
        {
            Assert.IsTrue (_apiTester.RemoveDoesNotThrowAnyExceptions());
        }

        [Test()]
        public void ApiTestTheExpectedDataIsReturnedFromGetForANonExistentKey()
        {
            Assert.IsTrue(_apiTester.TheExpectedDataIsReturnedFromGetForANonExistentKey());
        }
    }
}

