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
        private static string CreateAndReturnDataDirectory(string applicationName)
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + applicationName + "\\";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            return directory;
        }


        public Task<byte[]> ReadFileAsync(string applicationName, string contextInfo)
        {
            return Task.Run<byte[]>(() =>
                {
                    var directory = CreateAndReturnDataDirectory(applicationName);

                    var fileName = directory + contextInfo + ".json";
                    
                    if (System.IO.File.Exists(fileName))
                        return System.IO.File.ReadAllBytes(directory + contextInfo + ".json");
                    else
                        return null;
                    
                });
        }

        public Task SaveFileAsync(string applicationName, string contextInfo, byte[] data)
        {
            return Task.Run(() =>
                {
                    var directory = CreateAndReturnDataDirectory(applicationName);

                    System.IO.File.WriteAllBytes(directory + contextInfo + ".json", data);
                });
        }
    }
}
