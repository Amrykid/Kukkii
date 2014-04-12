using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kukkii.UnitTests
{
    public class HeavyTests
    {
        [Fact]
        public async Task InsertLotsOfRandomObjectsAndWaitAndGetOneOfThemAsync()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 20000; i++)
            {
                tasks.Add(
                    CookieJar.InMemory.InsertObjectAsync("Object" + i.ToString(), i * 2));
            }

            await Task.WhenAll(tasks.ToArray());

            Assert.Equal(26 * 2, (await CookieJar.InMemory.GetObjectAsync<int>("Object26")));

            Utilities.ForceResetCookieJar();
        }
        [Fact]
        public async Task InsertLotsOfRandomObjectsAndWaitFor5MinutesAndGetOneOfThemAsync()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 20000; i++)
            {
                tasks.Add(
                    CookieJar.InMemory.InsertObjectAsync("Object" + i.ToString(), i * 2));
            }

            await Task.WhenAll(tasks.ToArray());

            await Task.Delay(60000 * 5);

            Assert.Equal(26 * 2, (await CookieJar.InMemory.GetObjectAsync<int>("Object26")));

            Utilities.ForceResetCookieJar();
        }
    }
}
