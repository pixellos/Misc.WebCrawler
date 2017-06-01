using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebCrawler.Model;
using WebCrawler.Model.Benchmarking;

namespace WebCrawler.Controllers
{
    public class WebStatisticsController : Controller
    {
        private IWebSiteBenchmarker Benchmarker { get; }
        private ICrawler Crawler { get; }
        public WebStatisticsController(IWebSiteBenchmarker benchmarker, ICrawler crawler)
        {
            this.Benchmarker = benchmarker;
            this.Crawler = crawler;
        }

        [HttpGet]
        public async Task<IActionResult> SingleSiteRaw([FromQuery]string url)
        {
            var result = await this.WhenUrlSuccesfullyParsed(url, this.SingleSiteRaw);
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> SiteHistory([FromQuery]string url)
        {
            var result = await this.WhenUrlSuccesfullyParsed(url, this.SiteHistory);
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> SiteAccessTime([FromQuery]string url)
        {
            var result = await this.WhenUrlSuccesfullyParsed(url, this.SiteAccessTime);
            return result;
        }

        [HttpGet]
        public async Task<IActionResult> MinMaxValues()
        {
            var minValues = await this.Benchmarker.Mins();
            var maxValues = await this.Benchmarker.Maxs();
            Func<Entry, IEnumerable<Entry>, MinMaxEntryViewModel> selector = (q, ps) =>
            {
                if (ps.Count() > 1)
                {
                    throw new InvalidOperationException("Data inconsistency.");
                }
                var vm = new MinMaxEntryViewModel(q.Uri.AbsoluteUri, q.RequestDuration.Milliseconds, ps.Single().RequestDuration.Milliseconds);
                return vm;
            };
            var result = minValues.GroupJoin(maxValues, x => x.Uri, x => x.Uri, selector)
                .OrderByDescending(x => x.SlowestResponse);
            return this.View(result);
        }

        private async Task<IActionResult> SingleSiteRaw(Uri uri)
        {
            var crawled = await this.Crawler.CrawlSingle(uri);
            var vm = new EntryWithDescendantsViewModel(crawled.Uri.AbsoluteUri, crawled.RequestDuration.Milliseconds, crawled.Descendants.Select(x => x.AbsoluteUri));
            return this.Json(vm);
        }

        private async Task<IActionResult> SiteHistory(Uri uri)
        {
            var data = await this.Benchmarker.HistoryOf(uri);
            var vms = data.Select(x => new EntryViewModel(x.Uri.AbsoluteUri, x.RequestDuration.Milliseconds))
                .OrderByDescending(x => x.Miliseconds);
            return this.View(vms);
        }

        private async Task<IActionResult> SiteAccessTime(Uri uri)
        {
            var data = await this.Benchmarker.BenchmarkSite(uri);
            var vms = data.Select(x => new EntryViewModel(x.Uri.AbsoluteUri, x.RequestDuration.Milliseconds))
                .OrderByDescending(x => x.Miliseconds);
            return this.View(vms);
        }

        private async Task<IActionResult> WhenUrlSuccesfullyParsed(string url, Func<Uri, Task<IActionResult>> func)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri parsedUri))
            {
                var result = await func(parsedUri);
                return result;
            }
            else
            {
                return this.BadRequest("Error");
            }
        }
    }
}