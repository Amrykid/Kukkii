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
            await CookieJar.InMemory.InsertObjectAsync("YourKey", 6);
            await CookieJar.InMemory.InsertObjectAsync("MyKey", 25);

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync("MyKey"));

            Utilities.ForceResetCookieJar();
        }

        [Fact]
        public async Task SavesPeeksAndRetrievesAsync()
        {
            await CookieJar.InMemory.InsertObjectAsync("YourKey", 6);
            await CookieJar.InMemory.InsertObjectAsync("MyKey", 25);

            Assert.Equal(6, await CookieJar.InMemory.PeekObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.PeekObjectAsync("MyKey"));

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync("MyKey"));

            Utilities.ForceResetCookieJar();
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

            Assert.False(await CookieJar.InMemory.ContainsObjectAsync("YourKey"));

            Utilities.ForceResetCookieJar();
        }
    }
}
