namespace Searchr.UI
{
    using System.Windows.Forms;
    using Searchr.Core;

    public static class UiExtensions
    {
        private static SearchResult SearchResult(this DataGridViewRow row) => row.Tag as SearchResult;

        public static string Folder(this DataGridViewRow row) => row.SearchResult().FullFolder;

        public static string FileName(this DataGridViewRow row) => row.SearchResult().FileName;

        public static string FullPath(this DataGridViewRow row) => row.SearchResult().FullPath;

        public static string Truncate(this string content, int maxLength)
        {
            var a = content.Trim();
            return a.Length > maxLength
                       ? a.Substring(0, maxLength) + "…"
                       : a;
        }
    }
}
