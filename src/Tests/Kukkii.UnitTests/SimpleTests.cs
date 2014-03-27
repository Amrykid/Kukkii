using System;
using System.Threading.Tasks;
using Xunit;

namespace Kukkii.UnitTests
{
    public class SimpleTests
    {
        static SimpleTests()
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

        [Fact]
        public async Task ClearsOutItemsAsync()
        {
            var yourString = "Some string";
            await CookieJar.InMemory.InsertObjectAsync("YourKey", yourString, 5000);

            Assert.Equal(yourString, await CookieJar.InMemory.PeekObjectAsync("YourKey"));

            await Task.Delay(5000);

            Assert.Equal(yourString, await CookieJar.InMemory.PeekObjectAsync("YourKey"));

            await CookieJar.InMemory.CleanUpAsync();

            //need to fix this.
            Assert.False(await CookieJar.InMemory.ContainsObjectAsync("YourKey"));
        }
    }
}
