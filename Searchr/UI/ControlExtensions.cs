namespace System.Windows.Forms
{
    using Searchr.Core;

    public static class ControlExtensions
    {
        /// <summary>
        /// Calls Invoke on the control if it is required - use this in multi-threaded situations
        /// </summary>
        public static void InvokeAction<T>(this T control, Action<T> action) where T : Control
        {
            if (control.InvokeRequired)
            {
                try
                {
                    control.Invoke( new Action<T, Action<T>>(InvokeAction), control, action);
                }
                catch (ObjectDisposedException)
                {
                }
            }
            else
            {
                action(control);
            }
        }

        private static SearchResult? SearchResult(this DataGridViewRow row) => row!.Tag as SearchResult;

        public static string Folder(this DataGridViewRow row) => row!.SearchResult()!.FullFolder;

        public static string FileName(this DataGridViewRow row) => row!.SearchResult()!.FileName;

        public static string FullPath(this DataGridViewRow row) => row!.SearchResult()!.FullPath;

        public static string Truncate(this string content, int maxLength)
        {
            var a = content.Trim();
            return a.Length > maxLength
                       ? a.Substring(0, maxLength) + "…"
                       : a;
        }
    }
}
