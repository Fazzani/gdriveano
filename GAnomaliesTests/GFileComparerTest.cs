using GDrive.Anomalies;
using Google.Apis.Drive.v3.Data;
using Xunit;

namespace GAnomaliesTests
{
    public class GFileComparerTest
    {
        [Fact]
        public void CompareFileEqualTest()
        {
            var file = new File { Name = "FolderName", Id = "FolderID", Size = 1 };
            var file2 = new File { Name = "FolderName", Id = "Folder", Size = 1 };
            var compare = new GFileComparer();
            compare.Equals(file, file2);
            Assert.True(compare.Equals(file, file2));
        }

        [Fact]
        public void CompareFileNotEqualSizeTest()
        {
            var file = new File { Name = "FolderName", Id = "FolderID", Size = 1 };
            var file2 = new File { Name = "FolderName", Id = "Folder", Size = 2 };
            var compare = new GFileComparer();
            compare.Equals(file, file2);
            Assert.False(compare.Equals(file, file2));
        }

        [Fact]
        public void CompareFileNotEqualNameTest()
        {
            var file = new File { Name = "FolderName1", Id = "FolderID", Size = 1 };
            var file2 = new File { Name = "FolderName2", Id = "Folder", Size = 1 };
            var compare = new GFileComparer();
            compare.Equals(file, file2);
            Assert.False(compare.Equals(file, file2));
        }
    }
}