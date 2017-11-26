using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoSpaceSaver.Core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotoSpaceSaver.IntegrationTests.Core.Repository
{
    [TestClass]
    public class FileCacheRepositoryTests
    {
        [TestMethod]
        public void WhenOpenedWithNonExistingFileShouldReturnOpenedNewDb()
        {
            var sut = new FileCacheRepository();
            string path = @"C:\Project\TestData\PhotoSpaceSaverDb\NonExistingFile.db";
            try { 
                File.Delete(path);
            } catch (Exception)
            {
                // nop
            }
            Assert.AreEqual(FileCacheRepository.OpenResult.OpenedNewDb, sut.Open(path));
        }

        [TestMethod]
        public void WhenOpenedWithExistingValidDbShouldReturnOpenedExistingDb()
            {
            var sut = new FileCacheRepository();
            Assert.AreEqual(FileCacheRepository.OpenResult.OpenedExistingDb, sut.Open(@"C:\Project\TestData\PhotoSpaceSaverDb\ExistingDb.db"));
        }

        [TestMethod]
        public void WhenOpenedWithExistingInvalidFileShouldReturnOpenFailed()
        {
            var sut = new FileCacheRepository();
            Assert.AreEqual(FileCacheRepository.OpenResult.OpenFailed, sut.Open(@"C:\Project\TestData\PhotoSpaceSaverDb\ExistingInvalidDb.db"));
        }

        [TestMethod]
        public void WhenOpenedWithNonExistingDirectoryPathShouldReturnOpenFailed()
        {
            var sut = new FileCacheRepository();
            Assert.AreEqual(FileCacheRepository.OpenResult.OpenFailed, sut.Open(@"C:\Project\TestData\PhotoSpaceSaverDb\ThisDirectoryDoesNotExist\File.db"));
        }
    }
}
