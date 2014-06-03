using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.IO;

namespace Kukkii.UniversalApps
{
    public class UniversalFileSystemProvider: Kukkii.Core.ICookieFileSystemProvider
    {
        private static async Task<StorageFolder> CreateAndReturnDataDirectoryAsync(string applicationName, bool isLocal)
        {
            StorageFolder folder = isLocal ? Windows.Storage.ApplicationData.Current.LocalFolder : Windows.Storage.ApplicationData.Current.RoamingFolder;
            try
            {
                return await folder.GetFolderAsync(applicationName);
            }
            catch (Exception)
            {
            }

            return await folder.CreateFolderAsync(applicationName);
        }

        public async Task<byte[]> ReadFileAsync(string applicationName, string contextInfo, bool isLocal)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName, isLocal);
            try
            {
                var file = await folder.GetFileAsync(contextInfo + ".json");

                var accessStream = await file.OpenReadAsync();

                byte[] data = null;

                using (Stream stream = accessStream.AsStreamForRead((int)accessStream.Size))
                {
                    data = new byte[(int)stream.Length];
                    await stream.ReadAsync(data, 0, (int)stream.Length);
                }

                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task SaveFileAsync(string applicationName, string contextInfo, byte[] data, bool isLocal)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName, isLocal);
            StorageFile file = null;

            try
            {
                file = await folder.GetFileAsync(contextInfo + ".json");
            }
            catch (Exception) { }
            if (file == null)
                file = await folder.CreateFileAsync(contextInfo + ".json");

            var stream = await file.OpenTransactedWriteAsync();
            await stream.Stream.WriteAsync(data.AsBuffer());

            await stream.CommitAsync();

            stream.Dispose();
        }
    }
}
