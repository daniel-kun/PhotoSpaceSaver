using Microsoft.Identity.Client;
using Newtonsoft.Json;
using PhotoSpaceSaver.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PhotoSpaceSaver.Core.Logic
{
    public class OneDriveFileFinder
    {
        public class OneDriveSearchResult
        {
            public System.Net.HttpStatusCode httpStatusCode;
            public OneDriveSearchResultErrorItem error;
            [JsonProperty("@odata.nextLink")]
            public string oDataNextLink;
            [JsonProperty("@odata.deltaLink")]
            public string oDataDeltaLink;

            public List<OneDriveSearchResultItem> value;
        }

        public class OneDriveSearchResultItem
        {
            public string id;
            public string name;
            public OneDriveSearchResultItemDeletedState deleted;
            public OneDriveSearchResultFileItem file;
            public OneDriveSearchResultFolderItem folder;
            public List<OneDriveSearchResultItem> children;
        }

        public class OneDriveSearchResultItemDeletedState
        {
            public string state;
        }

        public class OneDriveSearchResultFolderItem
        {
            public int childCount;
        }

        public class OneDriveSearchResultFileItem
        {
            public OneDriveSearchResultFileHashes hashes;
        }

        public class OneDriveSearchResultFileHashes
        {
            public string crc32Hash;
            public string sha1Hash;
        }

        public class OneDriveSearchResultErrorItem
        {
            public string code;
            public string message;
        }

        public static OneDriveSearchResult ParseResult(string jsonResult)
        {
            var serializer = JsonSerializer.Create();
            using (var reader = new StringReader(jsonResult))
            {
                using (var jsonReader = new JsonTextReader(reader)) {
                    var result = serializer.Deserialize<OneDriveSearchResult>(jsonReader);
                    return result;
                }
            }
        }

        public class FileCacheItem
        {
            public FileCacheItemType Type;
            public string Id;
            public string Name;
            public string Sha1Cache;
        }

        public enum FileCacheItemType
        {
            Added,
            Deleted
        }

        public class DeltaLinkService
        {
            public string deltaLink;
        }

        public IEnumerable<FileCacheItem> BuildFileCache(string itemPath, string accessToken, DeltaLinkService deltaLinkService)
        {
            var oneDriveResult = OneDriveGetAsync(accessToken, deltaLinkService.deltaLink == null ? $"https://graph.microsoft.com/v1.0/me/drive/{itemPath}/delta?$select=id,name,file,deleted" : deltaLinkService.deltaLink).Result;
            while (oneDriveResult != null && oneDriveResult.httpStatusCode != System.Net.HttpStatusCode.NotFound)
            {
                if (oneDriveResult.error != null)
                {
                    throw new Exception(oneDriveResult.error.code + ": " + oneDriveResult.error.message);
                }
                if (oneDriveResult.value != null)
                {
                    foreach (var value in oneDriveResult.value)
                    {
                        if (value.deleted != null)
                        {
                            yield return new FileCacheItem()
                            {
                                Type = FileCacheItemType.Deleted,
                                Id = value.id,
                                Name = value.name,
                                Sha1Cache = null
                            };
                        }
                        else if (value.file != null && value.file.hashes != null)
                        {
                            yield return new FileCacheItem()
                            {
                                Type = FileCacheItemType.Added,
                                Id = value.id,
                                Name = value.name,
                                Sha1Cache = value.file.hashes.sha1Hash
                            };
                        }
                        else
                        {
                            // Not a file, maybe a folder
                        }
                    }
                }
                if (oneDriveResult.oDataDeltaLink != null)
                {
                    deltaLinkService.deltaLink = oneDriveResult.oDataDeltaLink;
                }
                if (oneDriveResult.oDataNextLink != null)
                {
                    oneDriveResult = OneDriveGetAsync(accessToken, oneDriveResult.oDataNextLink).Result;
                }
                else
                {
                    oneDriveResult = null;
                }
            }
        }

        public async Task<OneDriveSearchResult> OneDriveGetAsync(string accessToken, string queryUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var message = await client.GetAsync(queryUrl, HttpCompletionOption.ResponseContentRead);
            var body = await message.Content.ReadAsStringAsync();
            var result = ParseResult(body);
            result.httpStatusCode = message.StatusCode;
            return result;
        }

        public async Task<FilePropertiesModel> FindFileOnOneDrive(string fileHash, string accessToken)
        {
            return null;
        }
    }
}
