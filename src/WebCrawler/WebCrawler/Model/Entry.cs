using System;
using System.Collections.Generic;

namespace WebCrawler.Model
{
    public class Entry
    {
        public Entry()
        {
        }
        
        public Entry(Uri uri, IEnumerable<Uri> content, TimeSpan duration)
        {
            this.Uri = uri;
            this.Descendants = content;
            this.RequestDuration = duration;
        }

        public Uri Uri { get; set; }
        public IEnumerable<Uri> Descendants { get; set; }
        public TimeSpan RequestDuration { get;set; }
    }
}