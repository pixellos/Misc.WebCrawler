using Pixel.DataSource;
using System;
using System.Collections.Generic;

namespace WebCrawler.Model.Benchmarking
{
    public class DatabaseEntry: DatabaseModel
    {
        public Uri Uri { get; set; }
        public IEnumerable<Uri> ContentUris { get; set; }
        public TimeSpan Duration { get; set; }
    }
}