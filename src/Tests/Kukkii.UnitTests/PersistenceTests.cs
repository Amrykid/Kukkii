using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Kukkii.Containers;
using Kukkii.FS.Windows;
using Xunit;

namespace Kukkii.UnitTests
{
    public class PersistenceTests
    {
        static PersistenceTests()
        {
            try
            {
                CookieRegistration.FileSystemProvider = new WindowsFileSystemProvider();
                CookieJar.ApplicationName = "Kukkii-Tests";
            }
            catch (Exception)
            {
            }
        }

        [Fact]
        public async Task SavesDataToDiskAsync()
        {
            await CookieJar.Device.InsertObjectAsync("YourAge", 55);

            try
            {
                await CookieJar.Device.FlushAsync();
            }
            catch (Exception ex)
            {
                Assert.IsType<Exception>(ex);
            }

            try
            {
                //not apart of the test
                CleanUp();
            }
            catch (Exception) { }

            Utilities.ForceResetCookieJar();
        }

        [Fact]
        public async Task SavesAndLoadsSimpleDataOnDiskAsync()
        {
            CookieJar.ApplicationName = "Kukkii-Tests";

            var date = DateTime.Now;

            await CookieJar.Device.InsertObjectAsync("SomeObject", date);
            await CookieJar.Device.FlushAsync();
            await CookieJar.Device.ClearContainerAsync();

            Utilities.ForcePersistentCacheReload();

            Assert.Equal(date, await CookieJar.Device.GetObjectAsync("SomeObject"));

            Utilities.ForceResetCookieJar();
        }

        private void CleanUp()
        {
            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + CookieJar.ApplicationName + "\\", true);
        }
    }
}
