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
        public WebStatisticsController(IWebSiteBenchmarker benchmarker)
        {
            this.Benchmarker = benchmarker;
        }

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
                var vm = new MinMaxEntryViewModel(q.Uri.AbsoluteUri, q.Duration.Milliseconds, ps.Single().Duration.Milliseconds);
                return vm;
            };
            var result = minValues.GroupJoin(maxValues, x => x.Uri, x => x.Uri, selector);
            return this.View(result);
        }

        public async Task<IActionResult> SiteHistory([FromQuery]string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri parsedUri))
            {
                var data = await this.Benchmarker.HistoryOf(parsedUri);
                var vms = data.Select(x => new EntryViewModel(x.Uri.AbsoluteUri, x.Duration.Milliseconds))
                    .OrderByDescending(x => x.Miliseconds);
                return this.View(vms);
            }
            else
            {
                return this.BadRequest("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SiteAccessTime([FromQuery]string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri parsedUri))
            {
                var data = await this.Benchmarker.BenchmarkSite(parsedUri);
                var vms = data.Select(x => new EntryViewModel(x.Uri.AbsoluteUri, x.Duration.Milliseconds))
                    .OrderByDescending(x => x.Miliseconds);
                return this.View(vms);
            }
            else
            {
                return this.BadRequest("Error");
            }
        }
    }
}