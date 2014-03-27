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
        }

        [Fact]
        public void AllowsCookieRegistrationChangesBeforeCookieJarInitialization()
        {
            Assert.DoesNotThrow(new Assert.ThrowsDelegate(() =>
            {
                CookieRegistration.FileSystemProvider = new Kukkii.FS.Windows.WindowsFileSystemProvider();
                CookieJar.ApplicationName = "Kukkii-Tests";
            }));
        }
    }
}
