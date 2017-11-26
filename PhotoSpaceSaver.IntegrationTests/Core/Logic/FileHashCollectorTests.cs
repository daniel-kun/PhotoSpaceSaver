using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PhotoSpaceSaver.IntegrationTests
{
    [TestClass]
    public class FileHashCollectorTests
    {
        [TestMethod]
        public void WhenRunForTestDataDirShouldListAllFiles()
        {
            var collector = new PhotoSpaceSaver.Core.Logic.FileHashCollector();
            var result = collector.CollectAsync(new List<string> { @"C:\Project\TestData\PhotoSpaceSaver\" }).Result;
            Assert.IsTrue(result.FindIndex(file => file.FileHash == "37c44d51649beed8d751eb42656e08f489e5516b") >= 0);
            Assert.IsTrue(result.FindIndex(file => file.FileHash == "ddf89af041536cb62e8d24afc6987329b7f67c0c") >= 0);

            Assert.IsTrue(
                result.FindIndex(file => 
                    file.FileHash == "0278131b548cc43409db325ffa35739f280b1051"
                    && file.FileName == "DSC_9938.jpg") >= 0);
            Assert.IsTrue(result.FindIndex(file => 
                    file.FileHash == "cec828332229d0efbc26ca8e8100820a614aa63d"
                    && file.FileName == "DSC_9928.jpg" 
                    && file.FileSize == 8271051L) >= 0);
            Assert.IsTrue(
                result.FindIndex(file => 
                    file.FileHash == "1afb845edb3a207e418ac8967fca6ae36b8ef58f"
                    && file.FileName == "DSC_9930.jpg"
                    && file.FileSize == 5324681L
                    && file.FullFileName == @"C:\Project\TestData\PhotoSpaceSaver\2014\2014-06 Luisa Fotoshooting\DSC_9930.jpg") >= 0);
        }
    }
}
