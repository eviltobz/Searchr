using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Searchr.Core
{
    public class SearchResponse
    {
        public BlockingCollection<SearchResult> Results { get; }
        public int Hits;
        public int Misses;
        public Exception? Error { get; private set; }
        public List<Exception> Errors { get; } = new List<Exception>();

        public SearchResponse()
        {
            this.Results = new BlockingCollection<SearchResult>();
        }

        public void SetError(Exception ex)
        {
            this.Error = ex;
            Errors.Add(ex);
        }
    }
}
