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

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync<int>("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync<int>("MyKey"));

            Utilities.ForceResetCookieJar();
        }

        [Fact]
        public async Task SavesPeeksAndRetrievesAsync()
        {
            await CookieJar.InMemory.InsertObjectAsync("YourKey", 6);
            await CookieJar.InMemory.InsertObjectAsync("MyKey", 25);

            Assert.Equal(6, await CookieJar.InMemory.PeekObjectAsync<int>("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.PeekObjectAsync<int>("MyKey"));

            Assert.Equal(6, await CookieJar.InMemory.GetObjectAsync<int>("YourKey"));
            Assert.Equal(25, await CookieJar.InMemory.GetObjectAsync<int>("MyKey"));

            Utilities.ForceResetCookieJar();
        }

        [Fact]
        public async Task ClearsOutItemsAsync()
        {
            var yourString = "Some string";
            await CookieJar.InMemory.InsertObjectAsync("YourKey", yourString, 5000);

            Assert.Equal(yourString, await CookieJar.InMemory.PeekObjectAsync<string>("YourKey"));

            await Task.Delay(5000);

            Assert.Equal(yourString, await CookieJar.InMemory.PeekObjectAsync<string>("YourKey"));

            await CookieJar.InMemory.CleanUpAsync();

            Assert.False(await CookieJar.InMemory.ContainsObjectAsync("YourKey"));

            Utilities.ForceResetCookieJar();
        }
    }
}
