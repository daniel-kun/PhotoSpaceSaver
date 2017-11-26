using PhotoSpaceSaver.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSpaceSaver.Core.Logic
{
    public class FileHashCollector
    {
        async public Task<List<FilePropertiesModel>> CollectAsync(List<string> folders)
        {
            return await Task.Run<List<FilePropertiesModel>>(() =>  CollectSync(folders));
        }

        private List<FilePropertiesModel> CollectSync(List<string> folders)
        {
            return folders.SelectMany<string, FilePropertiesModel>(folder => CollectFromFolderSync(folder)).ToList();
        }

        private IEnumerable<FilePropertiesModel> CollectFromFolderSync(string folder)
        {
            try
            {
                var dirInfo = new DirectoryInfo(folder);
                return from file in dirInfo.GetFiles("*.*", SearchOption.AllDirectories)
                       select CollectFromFileInfoSync(file);
            } catch (Exception)
            {
                return new List<FilePropertiesModel>();
            }
        }

        private FilePropertiesModel CollectFromFileInfoSync(FileInfo file)
        {
            var result = new FilePropertiesModel
            {
                FileName = file.Name,
                FullFileName = file.FullName,
                FileSize = 0,
                FileHash = string.Empty
            };
            try
            {
                result.FileSize = (ulong)file.Length;
                result.FileHash = CreateFileHashFromFileInfo(file);
            } catch (Exception)
            {
                // nop
            }
            return result;
        }

        private string CreateFileHashFromFileInfo(FileInfo file)
        {
            using (var hash = SHA1.Create())
            {
                using (var fileStream = File.OpenRead(file.FullName))
                {
                    return String.Concat(from hashDigit in hash.ComputeHash(fileStream) select hashDigit.ToString("x2"));
                }
            }
        }
    }
}
