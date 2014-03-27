using System;
using Xunit;

namespace Kukkii.UnitTests
{
    public class LogicTests
    {
        [Fact]
        public void PreventsCookieRegistrationChangesAfterCookieJarInitialization()
        {
            CookieJar.ApplicationName = "Kukkii-Tests";
            Assert.Throws<InvalidOperationException>(new Assert.ThrowsDelegate(() =>
                CookieRegistration.FileSystemProvider = new Kukkii.FS.Windows.WindowsFileSystemProvider()));

            Utilities.ForceResetCookieJar();
        }

        [Fact]
        public void AllowsCookieRegistrationChangesBeforeCookieJarInitialization()
        {
            Utilities.ForceResetCookieJar(false);

            //Assert.DoesNotThrow(() =>
            //{
            CookieRegistration.FileSystemProvider = new Kukkii.FS.Windows.WindowsFileSystemProvider();
            CookieJar.ApplicationName = "Kukkii-Tests";
            //});

            Utilities.ForceResetCookieJar();
        }
    }
}
