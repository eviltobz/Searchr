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
        public IReadOnlyCollection<ValueTuple<int, string>> Matches { get => matches; }
        private readonly List<ValueTuple<int, string>> matches = new List<ValueTuple<int, string>>();

        public SearchResult(FileInfo File, string searchRoot)
        {
            this.File = File;
            var full = File.Directory.FullName;
            RelativeFolder = full.Replace(searchRoot, ".");
        }

        private SearchResult() { }

        public string FileName { get => File.Name; }
        public string FullPath { get => File.FullName; }
        public string FileType { get => File.Extension; }
        public string FullFolder { get => File.Directory.FullName; }
        public string RelativeFolder { get; private set; }

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
