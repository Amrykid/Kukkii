using Kukkii.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Kukkii.FS.WP8
{
    public class WP8FileSystemProvider : ICookieFileSystemProvider
    {
        private static async Task<StorageFolder> CreateAndReturnDataDirectoryAsync(string applicationName)
        {
            try
            {
                return await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(applicationName);
            }
            catch (Exception)
            {
            }

            return await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(applicationName);
        }

        public async System.Threading.Tasks.Task<byte[]> ReadFileAsync(string applicationName, string contextInfo)
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

        public async System.Threading.Tasks.Task SaveFileAsync(string applicationName, string contextInfo, byte[] data)
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

            var stream = await file.OpenTransactedWriteAsync();
            await stream.Stream.WriteAsync(data.AsBuffer());

            await stream.CommitAsync();

            stream.Dispose();
        }
    }
}
