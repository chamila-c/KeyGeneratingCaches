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
        [Category("Excluded")]
        /// <summary>
        /// A lazy test of the Api, that runs all the tests in one go.
        /// Good for quick verification, but lacks any detail if particular tests fail
        /// </summary>
        public void PassesAllApiTest()
        {
            Assert.IsTrue (_apiTester.PassesAllApiTests ());
        }

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

        [Test()]
        public void ApiTestNullKeySuppliedToGetDoesNotResultInExceptions()
        {
            Assert.IsTrue (_apiTester.NullKeySuppliedToGetDoesNotResultInExceptions ());
        }

        [Test()]
        public void ApiTestEmptyKeySuppliedToGetDoesNotResultInExceptions()
        {
            Assert.IsTrue (_apiTester.EmptyKeySuppliedToGetDoesNotResultInExceptions ());
        }
    }
}

