using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static PhotoSpaceSaver.Core.Logic.PhotoSpaceSaver;
using PhotoSpaceSaver.Core.Logic;

namespace PhotoSpaceSaver.IntegrationTests.Core.Logic
{
    [TestClass]
    public class PhotosSpaceSaverTests
    {
        private class TestCallbackHandler : IPhotoSpaceSaverCallbacks
        {
            public List<string> deletedFiles = new List<string>();
            public List<string> skippedFiles = new List<string>();
            public bool? finishedSuccessful = null;

            public void FileDeleted(string fileName)
            {
                deletedFiles.Add(fileName);
            }

            public void FileSkipped(string fileName)
            {
            }

            public void Finished(bool successful)
            {
                finishedSuccessful = successful;
            }

            public void Progress(PhotoSpaceSaverState state, int progressInPercent)
            {
            }

            public void Started()
            {
            }
        }

        [TestMethod]
        public void WhenStartedWithTestDataDirShouldRemoveTwoFiles()
        {
            string targetDir = @"C:\Project\TestData\Temp\" + Guid.NewGuid().ToString();
            Directory.CreateDirectory(targetDir);

            List <string> fullFileList = CopyTestDataDir(@"C:\Project\TestData\PhotoSpaceSaver", targetDir);

            var sut = new PhotoSpaceSaver.Core.Logic.PhotoSpaceSaver();
            var testCallbackHandler = new TestCallbackHandler();
            sut.StartAsync(new List<string> { targetDir }, accessToken, testCallbackHandler).Wait();
            Assert.AreEqual(fullFileList.Count, testCallbackHandler.deletedFiles.Count);


            //Directory.Delete(targetDir, true);
        }

        /**
        Returns the full paths of all files that have been copied.
        **/
        private List<string> CopyTestDataDir(string SourceDir, string TargetDir)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourceDir, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourceDir, TargetDir));

            var result = new List<string>();
            //Copy all the files & Replaces any files with the same name
            foreach (string sourceFile in Directory.GetFiles(SourceDir, "*.*",
                SearchOption.AllDirectories))
            {
                var targetFile = sourceFile.Replace(SourceDir, TargetDir);
                result.Add(targetFile);
                File.Copy(sourceFile, targetFile, true);
            }
            return result;
        }

        private static string accessToken = "EwBAA8l6BAAU7p9QDpi/D7xJLwsTgCg3TskyTaQAAeRI+nYNEhW4eqDzmp9CPlpj3NAdJp/sXrnUIM2xnfBv+r9j0mDrQHnPvTRKfmshyNYwLT2RA9gqeakn8I9kJUazK5nKalF+dS1U8g3i7yuCQSNAhsZ1i1FgazSlOdLutQrN+m3tyA7+suZjDN4qAIY4RXC1DwYQa7AdZ8CvarQm56GotEyHIn0aGLnB8KN0WYbkz3cdibRUhyJXDAdPkPVP63huZhl/Tbyf9UzEgUSFL9yr2zvpG/nClQp3ouCmHVDDm+ghZXK1fEUC6jUs3lgETHdTqE/ZvAdldV6NU+5vtoh6lDW1iZucRAangHzi7VpzXYPnM0LXxUDfmprW0Z8DZgAACK2TKbv9yjV1EAIrxphfm7AF5Qb8Z2aGp9LTvfzIo+wCuGfHrSuOpHymSZ/Jcc7i/jF84he6O2V0mLC3Zwa55yT5fL3bJapa1J3ePSr+Vii2wq3gmR1+UsrbZiGaSDtpGdZXhg4jF0K7OZD0GQKoB4IPlUqCPtkLEWHSHf0HNvrPa3RDg9UwsbpNvltTzubnyXChyDmEwvxEPM1WdaSzF46k3BIQ6sJbq41PXJVhIJmpaHrUDcKldUFDK74zhQl+oiiD3Pt83KN7xGPmGdSXfXGDJcb4U8P5Cy5AFVbrXVXKHltwnb8bdy9T9WpLL3+I5gFOQLmBq+GnAX1EaKBxezhvvxms32jx5NmArtluUxPoSzvZJhaAaFgdE/EQ1+1XuiSfLzktQvNnLNE2nbbDhUnGKM5ZlckfA7vRZUqzilcCpeRxhgBKTlBX1wPY5l8fcptpIupaetoHFC5WX817buDgWahTzi/lFCZgsETLy3dkW2wV8UCy1Jzdpgf1qNP0elX2Zdr4B6f4Zpehn+OL/R+HaynX1ZXvqALyvpNd9JaHmaBhHsk1pD3BF4NBtU8BXrB/l9FljMhERnqnUxinqF8IiAYrrxyGvl9DNeIHILtOXADwkOLeGIPxbLIZWwzhq1UV2tK7gxuFskuJ5MUEQdPHlCnAiWVRppJcGrIjgzTu30R2XQbCiEPtPWIIugOwdX0uht+i2jBWb/U/Ag==";
    }
}
