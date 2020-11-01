using GDrive.Anomalies;
using Google.Apis.Drive.v3.Data;
using Xunit;

namespace GAnomaliesTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void AssertGDriveFolderToText()
        {
            var file = new File { Name = "FolderName", Id = "FolderID", MimeType = "application/vnd.google-apps.folder" };
            Assert.StartsWith("Folder", file.ToText());
        }

        [Fact]
        public void NormalizeGFileNameTest()
        {
            var folderName = "FolderName";
            var file = new File { Name = $"{GFileStrictComparer.CopyString}{folderName}", Id = "FolderID", MimeType = "application/vnd.google-apps.folder" };
            var name = file.NormalizeGFileName();
            Assert.StartsWith(folderName, name);
        }
    }
}
