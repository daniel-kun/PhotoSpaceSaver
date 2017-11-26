using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PhotoSpaceSaver.Core.Logic.OneDriveFileFinder;

namespace PhotoSpaceSaver.Core.Repository
{
    /**
    FileCache is used to build and store a cache of file infos that have been retrieved from OneDrive in delta-chunks,
    and look up whether a local file is in that cache, which should mean that it is on OneDrive.

    The cache is stored in a local SQLite file.
    **/
    public class FileCacheRepository : IDisposable
    {
        public enum OpenResult
        {
            OpenedExistingDb,
            OpenedNewDb,
            OpenFailed
        }

        private SQLiteConnection _sqlConnection;

        /**
        Opens the SQLite cache file. Returns true when a new file has been created.
        In this case, the cache needs to be re-built.
        **/
        public OpenResult Open(string dbFilePath)
        {
            try
            {
                bool fileExisted = File.Exists(dbFilePath);
                _sqlConnection = new SQLiteConnection(dbFilePath);
                var columns = _sqlConnection.GetTableInfo("blubb");
                if (fileExisted)
                {
                    return OpenResult.OpenedExistingDb;
                }
                else
                {
                    return OpenResult.OpenedNewDb;
                }
            } catch (SQLiteException)
            {
                return OpenResult.OpenFailed;
            }
        }

        /**
        Updates the cache with the given FileCacheItems. Items of type Added are added or updated,
        items of type Deleted are removed from the cache.
        Updates affect calls to Lookup(string) immediately.
        **/
        public void UpdateCache(IEnumerable<FileCacheItem> fileCacheItems)
        {

        }

        /**
        Looks up a file in the cache. Returns true when the file could be found and false otherwise.
        **/
        public bool Lookup(string sha1Hash)
        {
            return false;
        }

        /**
        Removes all items from the cache. This should be done e.g. when no delta could be retrieved from
        the last cached state.
        This affects successive calls to Lookup(string) immediately.
        **/
        public void Clear()
        {

        }

        public void Close()
        {
            _sqlConnection.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
