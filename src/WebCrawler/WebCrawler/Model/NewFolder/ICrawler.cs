using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCrawler.Model.Benchmarking
{
    public interface ICrawler
    {
        Task<IEnumerable<Entry>> Crawl(Uri urlToCrawl);
        Task<Entry> CrawlSingle(Uri urlToCrawl);
    }
}