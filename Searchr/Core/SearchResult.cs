using System.Collections.Generic;
using System.IO;

namespace Searchr.Core
{
    using System;

    public class SearchResult
    {
        private FileInfo File { get; }
        public bool Match { get; private set; }
        public int TotalCount { get; private set; }
        public IReadOnlyCollection<ValueTuple<int, string>> Matches => matches;
        private readonly List<ValueTuple<int, string>> matches = new List<ValueTuple<int, string>>();

        public SearchResult(FileInfo file, string searchRoot)
        {
            this.File = file;
            var full = file.Directory.FullName;
            RelativeFolder = full.Replace(searchRoot, ".");
        }

        private SearchResult() { }

        public string FileName => File.Name;
        public string FullPath => File.FullName;
        public string FileType => File.Extension;
        public string FullFolder => File.Directory.FullName;
        public string RelativeFolder { get; }

        public long FileSize { get => (File.Length / 1024 + (File.Length % 1024 == 0 ? 0 : 1)); }


        public void IsPathMatch()
        {
            Match = true;
        }

        public void Add(int lineNumber, string content)
        {
            TotalCount++;
            matches.Add((lineNumber, content));
            Match = true;
        }

        public static readonly SearchResult Error = new SearchResult();
    }
}
