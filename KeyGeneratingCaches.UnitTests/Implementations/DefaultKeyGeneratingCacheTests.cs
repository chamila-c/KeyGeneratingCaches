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
        /// <summary>
        /// Tests API conformance using the KeyGeneratingCaches.Api.Verification.ApiTester
        /// class' 'PassesAllApiTests' method
        /// </summary>
        public void PassesAllApiTests()
        {
            Assert.IsTrue (_apiTester.PassesAllApiTests (this.GetType().FullName +  ".PassesAllApiTests"));
        }
    }
}

