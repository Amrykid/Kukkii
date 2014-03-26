using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kukkii.UnitTests
{
    [TestClass]
    public class SimpleTests
    {
        public SimpleTests()
        {
            CookieJar.ApplicationName = "UnitTests";
        }
        [TestMethod]
        public async void SaveAndRetrieveAsyncTest()
        {
            Assert.IsTrue(await CookieJar.InMemory.InsertObjectAsync("YourKey", 6), "Insert Test #1");
            Assert.IsTrue(await CookieJar.InMemory.InsertObjectAsync("MyKey", 25), "Insert Test #2");

            Assert.AreEqual(6, await CookieJar.InMemory.GetObjectAsync("YourKey"));
            Assert.AreEqual(25, await CookieJar.InMemory.GetObjectAsync("MyKey"));
        }
    }
}
