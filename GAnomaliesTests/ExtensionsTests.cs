using Google.Apis.Drive.v3.Data;
using Xunit;

namespace GDrive.Anomalies.Library.Tests
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
            const string folderName = "FolderName";
            var file = new File { Name = $"{GFileStrictComparer.CopyString}{folderName}", Id = "FolderID", MimeType = "application/vnd.google-apps.folder" };
            var name = file.NormalizeGFileName();
            Assert.StartsWith(folderName, name);
        }
    }
}