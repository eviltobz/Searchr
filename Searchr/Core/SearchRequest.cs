//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Searchr.Core
{
    using System.Text.Json.Serialization;

    public class SearchRequest
    {
        public string Directory { get; set; }
        public SearchOption DirectoryOption { get; set; }
        public string SearchTerm { get; set; }
        public SearchMethod SearchMethod { get; set; } = SearchMethod.SingleLine;
        public bool MatchCase { get; set; }
        public int ParallelSearches { get; set; }
        public List<string> ExcludeFileWildcards { get; set; }
        public List<string> IncludeFileWildcards { get; set; }
        public List<string> ExcludeFolderNames { get; set; }
        public bool ExcludeHidden { get; set; }
        public bool ExcludeSystem { get; set; }
        public bool ExcludeBinaryFiles { get; set; }
        public bool SearchFileContents { get; set; }
        public bool SearchFileName { get; set; }
        public bool SearchFilePath { get; set; }
        public SearchRequest? ParentSearch { get; set; }

        private CancellationTokenSource cancellationSource;

        [JsonIgnore]
        public CancellationToken CancellationToken => cancellationSource.Token;

        public bool Aborted => CancellationToken.IsCancellationRequested;

        public SearchRequest(
            string directory,
            SearchOption directoryOption,
            string searchTerm,
            SearchMethod searchMethod,
            bool matchCase,
            int parallelSearches,
            List<string> excludeFileWildcards,
            List<string> includeFileWildcards,
            List<string> excludeFolderNames,
            bool excludeSystem,
            bool excludeHidden,
            bool excludeBinaryFiles,
            bool searchFileContents,
            bool searchFileName,
            bool searchFilePath,
            SearchRequest? parentSearch)
        {
            Directory = directory;
            DirectoryOption = directoryOption;
            SearchTerm = searchTerm;
            SearchMethod = searchMethod;
            MatchCase = matchCase;
            ParallelSearches = parallelSearches;
            ExcludeFileWildcards = excludeFileWildcards;
            IncludeFileWildcards = includeFileWildcards;
            ExcludeFolderNames = excludeFolderNames;
            ExcludeSystem = excludeSystem;
            ExcludeHidden = excludeHidden;
            ExcludeBinaryFiles = excludeBinaryFiles;
            SearchFileContents = searchFileContents;
            SearchFileName = searchFileName;
            SearchFilePath = searchFilePath;
            ParentSearch = parentSearch;

            cancellationSource = new CancellationTokenSource();
        }

        //public SearchRequest()
        //{
        //    Directory = null;
        //    DirectoryOption = SearchOption.AllDirectories;
        //    SearchTerm = null;
        //    SearchMethod = SearchMethod.SingleLine;
        //    MatchCase = false;
        //    ParallelSearches = 4;
        //    ExcludeFileWildcards = new List<string>();
        //    IncludeFileWildcards = new List<string>();
        //    ExcludeFolderNames = new List<string>();
        //    ExcludeHidden = false;
        //    ExcludeSystem = false;
        //    ExcludeBinaryFiles = false;
        //    SearchFileContents = true;
        //    SearchFileName = false;
        //    SearchFilePath = false;

        //    cancellationSource = new CancellationTokenSource();
        //}
        public void Abort()
        {
            this.cancellationSource.Cancel();
        }

        [JsonIgnore]
        public SearchAlgorithm Algorithm
        {
            get
            {
                switch (SearchMethod)
                {
                    case SearchMethod.SingleLine: return SearchAlgorithm.SingleLine;
                    case SearchMethod.SingleLineRegex: return SearchAlgorithm.SingleLineRegex;
                    default: throw new NotImplementedException($"Algorithm for Search Method {SearchMethod} has not been implemented");
                }
            }
        }
    }
}
