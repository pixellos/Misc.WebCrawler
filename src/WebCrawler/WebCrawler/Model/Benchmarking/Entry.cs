using System;
using System.Collections.Generic;

namespace WebCrawler.Model.Benchmarking
{
    public class Entry
    {
        public Entry()
        {
        }
        
        public Entry(Uri uri, IEnumerable<Uri> content, TimeSpan duration)
        {
            this.Uri = uri;
            this.ContentUris = content;
            this.Duration = duration;
        }

        public Uri Uri { get; set; }
        public IEnumerable<Uri> ContentUris { get; set; }
        public TimeSpan Duration { get;set; }
    }
}