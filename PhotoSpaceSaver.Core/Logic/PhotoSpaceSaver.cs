using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using PhotoSpaceSaver.Core.Models;

namespace PhotoSpaceSaver.Core.Logic
{
    public class PhotoSpaceSaver
    {
        public enum PhotoSpaceSaverState
        {
            Scanning,
            Deleting
        }

        public interface IPhotoSpaceSaverCallbacks
        {
            void Started();

            void Progress(PhotoSpaceSaverState state, int progressInPercent);
            void FileSkipped(string fileName);
            void FileDeleted(string fileName);

            void Finished(bool successful);
        }

        public async Task StartAsync(List<String> directories, string oneDriveAccessToken, IPhotoSpaceSaverCallbacks callbacks)
        {
            callbacks.Started();
            var oneDriveFileFinder = new OneDriveFileFinder();
            var localFiles = await fileHashCollector.CollectAsync(directories);
            foreach (var localFile in localFiles)
            {
                FilePropertiesModel oneDriveFile = oneDriveFileFinder.FindFileOnOneDrive(localFile.FileHash, oneDriveAccessToken).Result;
                if (oneDriveFile != null && oneDriveFile.FileName == localFile.FileName && oneDriveFile.FileHash == localFile.FileHash)
                {
                    File.Delete(localFile.FullFileName);
                    callbacks.FileDeleted(localFile.FileName);
                }
                else
                {
                    callbacks.FileSkipped(localFile.FileName);
                }
            }
            callbacks.Finished(true);
        }

        private FileHashCollector fileHashCollector = new FileHashCollector();
    }
}
