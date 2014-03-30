using Kukkii.Core;
using System;
using System.Collections.Generic;
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
            var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(applicationName);
            if (folder == null)
                folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(applicationName);

            return folder;
        }

        public async System.Threading.Tasks.Task<byte[]> ReadFileAsync(string applicationName, string contextInfo)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName);
            var file = await folder.GetFileAsync(contextInfo + ".json");

            if (file != null)
            {
                var stream = await file.OpenReadAsync();

                var buffer = new Windows.Storage.Streams.Buffer((uint)stream.Size);

                await stream.ReadAsync(buffer, (uint)buffer.Length, InputStreamOptions.ReadAhead);

                stream.Dispose();

                return buffer.ToArray();
            }
            else
                return null;
        }

        public async System.Threading.Tasks.Task SaveFileAsync(string applicationName, string contextInfo, byte[] data)
        {
            var folder = await CreateAndReturnDataDirectoryAsync(applicationName);
            var file = await folder.GetFileAsync(contextInfo + ".json");

            if (file == null)
                file = await folder.CreateFileAsync(contextInfo + ".json");


            var stream = await file.OpenTransactedWriteAsync();
            await stream.Stream.WriteAsync(data.AsBuffer());

            await stream.CommitAsync();

            stream.Dispose();
        }
    }
}
