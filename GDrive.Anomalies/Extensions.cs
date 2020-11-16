using System;
using Google.Apis.Drive.v3.Data;

namespace GDrive.Anomalies
{
    public static class Extensions
    {
        public const string CopyString = "Copie de ";

        public static string NormalizeGFileName(this File file)
        {
            return file.Name.LastIndexOf(CopyString) >= 0 ? file.Name[CopyString.Length..] : file.Name;
        }

        public static bool IsFolder(this File file)
        {
            return file?.MimeType?.Equals("application/vnd.google-apps.folder", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public static string ToText(this File file)
            => file.IsFolder() ? $"Folder ======== { file.Name} ({file.Id})" : $"{ file.Name} ({file.Id})";
    }
}