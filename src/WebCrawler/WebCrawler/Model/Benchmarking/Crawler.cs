using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using HtmlAgilityPack;
using System;
using System.IO;
using WebCrawler.Helpers;

namespace WebCrawler.Model.Benchmarking
{

    public class Crawler : ICrawler
    {
        private static string[] DefaultExtensions => new[] { "aspx", "html", "php", "htm", string.Empty };
        private string[] AllowedExtensions { get; }

        public Crawler() : this(Crawler.DefaultExtensions)
        {

        }

        public Crawler(string[] visiteableExtensions)
        {
            this.AllowedExtensions = visiteableExtensions;
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
            return resultData;
        }

        public async Task<Entry> CrawlSingle(Uri urlToCrawl)
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