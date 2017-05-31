using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using HtmlAgilityPack;
using System;
using System.IO;
using Pixel.DataSource;
using WebCrawler.Helpers;

namespace WebCrawler.Model.Benchmarking
{
    public class Crawler : IWebSiteBenchmarker
    {
        async Task<IEnumerable<Entry>> IWebSiteBenchmarker.BenchmarkSite(Uri site) => await this.Crawl(site);
        async Task<IEnumerable<Entry>> IWebSiteBenchmarker.HistoryOf(Uri uri) => this.HistoryOf(uri);
        private string[] AllowedExtensions { get; }
        private static string[] DefaultExtensions => new[] { "aspx", "html", "php", "htm", string.Empty };
        private HttpClient Client { get; }
        private IRepository<DatabaseEntry> Repository { get; }

        public Crawler(IRepository<DatabaseEntry> source, string[] extensions)
        {
            this.Client = new HttpClient();
            this.Repository = source;
            this.AllowedExtensions = extensions;
        }

        public Crawler(IRepository<DatabaseEntry> source) : this(source, Crawler.DefaultExtensions)
        {

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

        public async Task<IEnumerable<Entry>> Crawl(Uri urlToCrawl)
        {
            var visitedUrls = new HashSet<string>();
            var inputData = new Stack<Uri>();
            var resultData = new Stack<Entry>();
            inputData.Push(urlToCrawl);
            while (inputData.Any())
            {
                var entry = inputData.Pop();
                var itemUri = entry.AbsoluteUri;
                var lastAdressPart = itemUri.Reverse().TakeWhile(x => x != '/');
                var uri = entry.GetComponents(UriComponents.Host | UriComponents.Scheme | UriComponents.Path, UriFormat.Unescaped);
                var ext = Path.GetExtension(entry.Segments.Last());
                if (!visitedUrls.Contains(uri) && this.AllowedExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase))
                {
                    visitedUrls.Add(uri);
                    var notDynamicUri = new Uri(uri);
                    try
                    {
                        var crawled = await this.CrawlSingle(notDynamicUri);
                        resultData.Push(crawled);
                        var toPush = crawled.ContentUris
                            .Where(x => x.Host == urlToCrawl.Host)
                            .Select(x => x.SkipQuery())
                            .Where(x => !visitedUrls.Contains(x.AbsoluteUri))
                            .Where(x => !inputData.Contains(x));
                        foreach (var item in toPush)
                        {
                            inputData.Push(item);
                        }
                    }
                    //I know, this is ugly hack, but HAP authors forced me to do this coz they're not using derived exceptions... https://github.com/zzzprojects/html-agility-pack/blob/3fcf79abefdcedcc91f7e6add0a4f41ca4907495/src/HtmlAgilityPack.NETStandard/HtmlWeb.cs#L2003
                    catch (Exception ex) when (ex.Message.Equals("Error downloading html", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.WriteLine($"Error while processing: {urlToCrawl}");
                    }
                }
            }
            this.SaveResults(resultData);
            return resultData;
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

        internal async Task<Entry> CrawlSingle(Uri urlToCrawl)
        {
            var web = new HtmlWeb();
            var stopWatch = new Stopwatch();
            web.PreHandleDocument += (i) => stopWatch.Stop();
            stopWatch.Start();
            var page = await web.LoadFromWebAsync(urlToCrawl.AbsoluteUri, System.Text.Encoding.UTF8);
            var attributes = page.DocumentNode.Descendants().Where(x => x.Name.Equals("a", StringComparison.CurrentCultureIgnoreCase) && x.Attributes.Contains("href")).Select(x => x.GetAttributeValue("href", "D"));
            var uris = attributes.Where(x => x.FirstOrDefault() == '/').Select(x => new Uri(urlToCrawl, x));
            return new Entry(urlToCrawl, uris, stopWatch.Elapsed);
        }
    }
}