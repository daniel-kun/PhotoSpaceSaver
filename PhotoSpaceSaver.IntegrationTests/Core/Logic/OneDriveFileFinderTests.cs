using Microsoft.Identity.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PhotoSpaceSaver.Core.Logic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace PhotoSpaceSaver.IntegrationTests.Core.Logic
{
    [TestClass]
    public class OneDriveFileFinderTests
    {
        [TestMethod]
        public void WhenBuildingFileCacheForTestDataDirShouldReturn1998FilesAndDeltaLink()
        {
            var sut = new OneDriveFileFinder();
            var deltaLinkService = new OneDriveFileFinder.DeltaLinkService();
            var folders = sut.BuildFileCache("items/1F56D0165ED3C523%211018262", ReadTestAccessToken(), deltaLinkService).ToList();
            Assert.AreEqual(2004, folders.Count);
            Assert.IsNotNull(deltaLinkService.deltaLink);
        }

        [TestMethod]
        public void WhenUpdatingFileCacheForDynamicTestDataDirShouldRemoveDeletedFiles()
        {
            var sut = new OneDriveFileFinder();
            var deltaLinkService = new OneDriveFileFinder.DeltaLinkService();
            string accessToken = ReadTestAccessToken();
            // This file will be deleted:
            var oldFile = UploadStringAsFile(accessToken, "1F56D0165ED3C523%211021348", "test-old.txt", "Hello, World! I'm old");
            // This file will stay:
            var oldFile2 = UploadStringAsFile(accessToken, "1F56D0165ED3C523%211021348", "test-old2.txt", "Hello, World! I'm old");
            var files = sut.BuildFileCache("items/1F56D0165ED3C523%211021348", accessToken, deltaLinkService).ToList();
            Assert.AreEqual(2, files.Count);
            Assert.IsNotNull(deltaLinkService.deltaLink);
            var newFile = UploadStringAsFile(accessToken, "1F56D0165ED3C523%211021348", "test.txt", "Hello, World!");
            DeleteFile(accessToken, oldFile.id);

            var newFiles = sut.BuildFileCache("items/1F56D0165ED3C523%211021348", accessToken, deltaLinkService).ToList();
            DeleteFile(accessToken, newFile.id);
            DeleteFile(accessToken, oldFile2.id);
            var addedFilesList = (from addedFiles in newFiles where addedFiles.Type == OneDriveFileFinder.FileCacheItemType.Added select addedFiles).ToList(); 
            var deletedFilesList = (from deletedFiles in newFiles where deletedFiles.Type == OneDriveFileFinder.FileCacheItemType.Deleted select deletedFiles).ToList();
            Assert.AreEqual(1, addedFilesList.Count);
            Assert.AreEqual(1, deletedFilesList.Count);
            Assert.AreEqual(0, addedFilesList.FindIndex((item) => item.Id == newFile.id));
            Assert.AreEqual(0, deletedFilesList.FindIndex((item) => item.Id == oldFile.id));
        }

        [TestMethod]
        public void WhenFileExistsOnOneDriveShouldReturnTrue()
        {
            var sut = new OneDriveFileFinder();
            const string hash = "7940B926DE86F962FFF45416FBF89983DEC10335";
            Assert.AreEqual(hash, sut.FindFileOnOneDrive(hash, ReadTestAccessToken()).Result?.FileHash);
        }

        public static string ReadTestAccessToken()
        {
            string currentDir = Directory.GetCurrentDirectory();
            using (var reader = new StreamReader(currentDir + @"\..\..\..\TestAccessToken.txt"))
            {
                return reader.ReadToEnd();
            }
        }

        public static string DeleteFile(string accessToken, string itemId)
        {
            var request = WebRequest.Create($"https://graph.microsoft.com/v1.0/me/drive/items/{itemId}");
            request.Method = "DELETE";
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.ContentType = "application/json";
            var response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static OneDriveFileFinder.OneDriveSearchResultItem UploadStringAsFile(string accessToken, string parentId, string name, string content)
        {
            var fileContent = new MemoryStream();
            using (var writer = new StreamWriter(fileContent))
            {
                writer.WriteLine(content);
                writer.Flush();
                fileContent.Seek(0, SeekOrigin.Begin);
                return UploadFile(accessToken, parentId, name, fileContent, content.Length);
            }

        }

        public static OneDriveFileFinder.OneDriveSearchResultItem UploadFile(string accessToken, string parentId, string name, Stream content, long contentLength)
        {
            var request = WebRequest.Create($"https://graph.microsoft.com/v1.0/me/drive/items/{parentId}:/{name}:/content");
            request.Method = "PUT";
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.ContentType = "application/json";
            byte[] buffer = new byte[contentLength];
            int actualContentLength = content.Read(buffer, 0, (int)contentLength);
            request.ContentLength = actualContentLength;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, (int)actualContentLength);
            requestStream.Flush();
            requestStream.Close();
            var response = request.GetResponse();
            var json = JsonSerializer.Create();
            using (var textReader = new StreamReader(response.GetResponseStream()))
            {
                using (var jsonReader = new JsonTextReader(textReader))
                {
                    return json.Deserialize<OneDriveFileFinder.OneDriveSearchResultItem>(jsonReader);
                }
            }
        }


        [TestMethod]
        public void WhenParsingResultShouldReadFileNameAndSizeAndHashes()
        {
            var sut = OneDriveFileFinder.ParseResult(@"

{
    ""@odata.context"": ""https://graph.microsoft.com/v1.0/$metadata#users('d.albuschat%40gmail.com')/drive/root/$entity"",
    ""createdBy"": {
                ""user"": {
                    ""displayName"": ""Daniel Albuschat"",
            ""id"": ""1f56d0165ed3c523""
                }
            },
    ""createdDateTime"": ""2012-08-05T14:58:35.96Z"",
    ""cTag"": ""adDoxRjU2RDAxNjVFRDNDNTIzITI2MS42MzY0NTQ2NDA2ODUyMzAwMDA"",
    ""eTag"": ""aMUY1NkQwMTY1RUQzQzUyMyEyNjEuMw"",
    ""id"": ""1F56D0165ED3C523!261"",
    ""lastModifiedBy"": {
                ""application"": {
                    ""displayName"": ""OneDrive website"",
            ""id"": ""44048800""
                },
        ""user"": {
                    ""displayName"": ""Daniel Albuschat"",
            ""id"": ""1f56d0165ed3c523""
        }
            },
    ""lastModifiedDateTime"": ""2017-11-05T07:34:28.523Z"",
    ""name"": ""root"",
    ""parentReference"": {
                ""driveId"": ""1f56d0165ed3c523""
    },
    ""size"": 266077658681,
    ""webUrl"": ""https://onedrive.live.com/?cid=1f56d0165ed3c523"",
    ""fileSystemInfo"": {
                ""createdDateTime"": ""2012-08-05T14:58:35.96Z"",
        ""lastModifiedDateTime"": ""2015-04-28T05:51:38.857Z""
    },
    ""folder"": {
                ""childCount"": 13,
        ""view"": {
                    ""viewType"": ""thumbnails"",
            ""sortBy"": ""name"",
            ""sortOrder"": ""ascending""
        }
            },
    ""root"": {
            },
    ""children@odata.context"": ""https://graph.microsoft.com/v1.0/$metadata#users('d.albuschat%40gmail.com')/drive/root/children"",
    ""children"": [
        {
            ""id"": ""1F56D0165ED3C523!983221"",
            ""name"": ""Anwendungen"",
            ""size"": 113642572,
            ""folder"": {
                ""childCount"": 1,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
}
        },
        {
            ""id"": ""1F56D0165ED3C523!262"",
            ""name"": ""Bilder"",
            ""size"": 245574291607,
            ""folder"": {
                ""childCount"": 13,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""takenOrCreatedDateTime"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!264"",
            ""name"": ""Dokumente"",
            ""size"": 1231665789,
            ""folder"": {
                ""childCount"": 20,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!1006059"",
            ""name"": ""E-Mail-Anhänge"",
            ""size"": 0,
            ""folder"": {
                ""childCount"": 0,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!989941"",
            ""name"": ""Enpass"",
            ""size"": 159744,
            ""folder"": {
                ""childCount"": 1,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!22111"",
            ""name"": ""Musik"",
            ""size"": 19101867096,
            ""folder"": {
                ""childCount"": 98,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!263"",
            ""name"": ""Öffentlich"",
            ""size"": 6484067,
            ""folder"": {
                ""childCount"": 3,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!1002779"",
            ""name"": ""Rappelkiste"",
            ""size"": 37665459,
            ""folder"": {
                ""childCount"": 6,
                ""view"": {
                    ""viewType"": ""thumbnails"",
                    ""sortBy"": ""name"",
                    ""sortOrder"": ""ascending""
                }
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!538627"",
            ""name"": ""checklist web tool design.pdf"",
            ""size"": 318239,
            ""file"": {
                ""hashes"": {
                    ""crc32Hash"": ""40DD5AED"",
                    ""sha1Hash"": ""7940B926DE86F962FFF45416FBF89983DEC10335""
                },
                ""mimeType"": ""application/pdf""
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!190548"",
            ""name"": ""Einladungen Geburtstag 2015.xlsx"",
            ""size"": 9872,
            ""file"": {
                ""hashes"": {
                    ""crc32Hash"": ""3DEB1137"",
                    ""sha1Hash"": ""0E50C4C4BD9C436044F7F1E8A1313769697AA142""
                },
                ""mimeType"": ""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet""
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!1015508"",
            ""name"": ""KNX Bestellliste Ringstraße 6a.xlsx"",
            ""size"": 9227,
            ""file"": {
                ""hashes"": {
                    ""sha1Hash"": ""04E9DA58F724CE6940FAD967AFA753EF0C9E0AFD""
                },
                ""mimeType"": ""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet""
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!888918"",
            ""name"": ""Kosten Auto Mazda 6.xlsx"",
            ""size"": 10215,
            ""file"": {
                ""hashes"": {
                    ""crc32Hash"": ""1EAFFB4E"",
                    ""sha1Hash"": ""C1FE5FE8F79CA60B7A0C4A6F73AF9527EF9F19B5""
                },
                ""mimeType"": ""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet""
            }
        },
        {
            ""id"": ""1F56D0165ED3C523!1017413"",
            ""name"": ""x1-app(1).apk"",
            ""size"": 11534794,
            ""file"": {
                ""hashes"": {
                    ""sha1Hash"": ""64177F899C1E9972EA26BF1A57F16D7B9E5817C8""
                },
                ""mimeType"": ""application/vnd.android.package-archive""
            }
        }
    ]
}
");
            /*
            var file = sut.children.Find((item) => item.file?.hashes?.sha1Hash == "7940B926DE86F962FFF45416FBF89983DEC10335");
            Assert.IsNotNull(file);
            if (file != null)
            {
                Assert.AreEqual(file.name, "checklist web tool design.pdf");
                Assert.AreEqual(file.file.hashes.sha1Hash, "7940B926DE86F962FFF45416FBF89983DEC10335");
                Assert.AreEqual(file.size, 318239ul);
            }
            */
        }

    }
}
