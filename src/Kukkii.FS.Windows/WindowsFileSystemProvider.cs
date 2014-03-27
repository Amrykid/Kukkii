using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kukkii.Core;

namespace Kukkii.FS.Windows
{
    public class WindowsFileSystemProvider : ICookieFileSystemProvider
    {
        public byte[] ReadFile(string applicationName, string contextInfo)
        {
            var directory = CreateAndReturnDataDirectory(applicationName);

            try
            {
                return System.IO.File.ReadAllBytes(directory + contextInfo + ".json");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SaveFile(string applicationName, string contextInfo, byte[] data)
        {
            var directory = CreateAndReturnDataDirectory(applicationName);

            System.IO.File.WriteAllBytes(directory + contextInfo + ".json", data);
        }

        private static string CreateAndReturnDataDirectory(string applicationName)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + applicationName + "\\";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            return directory;
        }
    }
}
