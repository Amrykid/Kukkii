using System;
using System.Threading.Tasks;
using Xunit;

namespace Kukkii.UnitTests
{
    public class SimpleTests
    {
        public SimpleTests()
        {
            CookieJar.ApplicationName = "UnitTests";
        }
        [Fact]
        public async Task SavesAndRetrievesAsync()
        {
            bool test1 = await CookieJar.InMemory.InsertObjectAsync("YourKey", 6);
            bool test2 = await CookieJar.InMemory.InsertObjectAsync("MyKey", 25);

            Assert.True(test1, "Insert Test #1");
            Assert.True(test2, "Insert Test #2");

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync("MyKey"));
        }

        [Fact]
        public async Task SavesPeeksAndRetrievesAsync()
        {
            bool test1 = await CookieJar.InMemory.InsertObjectAsync("YourKey", 6);
            bool test2 = await CookieJar.InMemory.InsertObjectAsync("MyKey", 25);

            Assert.True(test1, "Insert Test #1");
            Assert.True(test2, "Insert Test #2");

            Assert.Equal(6, await CookieJar.InMemory.PeekObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.PeekObjectAsync("MyKey"));

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync("MyKey"));
        }
    }
}
