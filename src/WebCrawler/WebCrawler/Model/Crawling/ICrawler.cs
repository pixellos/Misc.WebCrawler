using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCrawler.Model.Crawling
{
    public interface ICrawler
    {
        Task<IEnumerable<Entry>> Crawl(Uri urlToCrawl);
        Task<Entry> CrawlSingle(Uri urlToCrawl);
    }
}