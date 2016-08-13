using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace Kukkii.UniversalApps
{
    public class UniversalFileSystemProvider : Kukkii.Core.ICookieFileSystemProvider
    {
        private StorageFolder rootFolder = null;
        internal UniversalFileSystemProvider(bool isLocal = false)
        {
            rootFolder = isLocal ? Windows.Storage.ApplicationData.Current.LocalFolder : Windows.Storage.ApplicationData.Current.RoamingFolder;
        }

        internal UniversalFileSystemProvider(StorageFolder baseFolder)
        {
            rootFolder = baseFolder;
        }

        private async Task<StorageFolder> CreateAndReturnDataDirectoryAsync(string applicationName)
        {
            try
            {
                return await rootFolder.GetFolderAsync(applicationName);
            }
            catch (Exception)
            {
            }

            return await rootFolder.CreateFolderAsync(applicationName);
        }

        public async Task DeleteFileAsync(string applicationName, string contextInfo)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName);
            try
            {
                var file = await folder.GetFileAsync(contextInfo + ".json");

                await file.DeleteAsync();
            }
            catch (Exception)
            {
               
            }
        }

        public async Task<byte[]> ReadFileAsync(string applicationName, string contextInfo)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName);
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

        public async Task SaveFileAsync(string applicationName, string contextInfo, byte[] data)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName);
            StorageFile file = null;

            try
            {
                file = await folder.GetFileAsync(contextInfo + ".json");
            }
            catch (Exception) { }
            if (file == null)
                file = await folder.CreateFileAsync(contextInfo + ".json");

            var stream = await file.OpenStreamForWriteAsync();

            await stream.WriteAsync(data, 0, data.Length);

            await stream.FlushAsync();

            stream.Dispose();
        }
    }
}
