using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System;
using Pixel.DataSource;
using WebCrawler.Helpers;
using WebCrawler.Model.Crawling;

namespace WebCrawler.Model.Benchmarking
{
    public class Benchmarker : IWebSiteBenchmarker
    {
        async Task<IEnumerable<Entry>> IWebSiteBenchmarker.HistoryOf(Uri uri) => this.HistoryOf(uri);
        private HttpClient Client { get; }
        private IRepository<DatabaseEntry> Repository { get; }
        private ICrawler Crawler { get; }

        public Benchmarker(IRepository<DatabaseEntry> source, ICrawler crawler)
        {
            this.Client = new HttpClient();
            this.Crawler = crawler;
            this.Repository = source;
        }

        public async Task<IEnumerable<Entry>> BenchmarkSite(Uri site)
        {
            var crawled = await this.Crawler.Crawl(site);
            this.SaveResults(crawled);
            return crawled;
        }

        public Task<IEnumerable<Entry>> Mins()
        {
            var databaseEntries = this.Repository.GroupBy(x => x.Uri);
            var minimumEntries = databaseEntries.Select(x => x.First(f => f.Duration <= x.Min(m => m.Duration)));
            var entries = minimumEntries.Select(x => x.InjectTo<Entry>());
            return Task.FromResult(entries);
        }

        public Task<IEnumerable<Entry>> Maxs()
        {
            var databaseEntries = this.Repository.GroupBy(x => x.Uri);
            var minimumEntries = databaseEntries.Select(x => x.First(f => f.Duration <= x.Max(m => m.Duration)));
            var entries = minimumEntries.Select(x => x.InjectTo<Entry>());
            return Task.FromResult(entries);
        }

        public IEnumerable<Entry> HistoryOf(Uri uri)
        {
            var result = this.Repository.Where(x => x.Uri.Equals(uri)).Select(x => x.InjectTo<Entry>());
            return result;
        }


        internal void SaveResults(IEnumerable<Entry> entries)
        {
            foreach (var item in entries)
            {
                var databaseItem = item.InjectTo<DatabaseEntry>();
                var result = this.Repository.Add(databaseItem);
                if (result is Pixel.Results.IError error)
                {
                    throw new InvalidOperationException(error?.Message);
                }
            }
        }

    }
}