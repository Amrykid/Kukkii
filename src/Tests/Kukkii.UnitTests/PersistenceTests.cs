using System;
using System.IO;
using System.Threading.Tasks;
using Kukkii.FS.Windows;
using Xunit;

namespace Kukkii.UnitTests
{
    public class PersistenceTests
    {
        static PersistenceTests()
        {
            CookieRegistration.FileSystemProvider = new WindowsFileSystemProvider();
            CookieJar.ApplicationName = "Kukkii-Tests";
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

            CleanUp();
        }

        private void CleanUp()
        {
            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + CookieJar.ApplicationName + "\\", true);
        }
    }
}
