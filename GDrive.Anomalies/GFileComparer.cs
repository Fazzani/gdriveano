using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Google.Apis.Drive.v3.Data;

namespace GDrive.Anomalies
{
    public class GFileComparer : IEqualityComparer<File>
    {
        public bool Equals([DisallowNull] File x, [DisallowNull] File y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) && x.Size == y.Size;
        }

        public int GetHashCode([DisallowNull] File obj) => obj.Id.GetHashCode();
    }

    /// <summary>
    /// Copy detected and check by size and file extension
    /// </summary>
    public class GFileStrictComparer : IEqualityComparer<File>
    {
        public const string CopyString = "Copie de ";

        /// <summary>
        /// Copy detected and check by size and file extension
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals([DisallowNull] File x, [DisallowNull] File y)
        {
            if (x.FileExtension?.Equals(y.FileExtension, StringComparison.OrdinalIgnoreCase) == false)
                return false;

            return x.Size == y.Size && y.NormalizeGFileName().Equals(x.NormalizeGFileName(), StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] File obj) => obj.Id.GetHashCode();
    }
}