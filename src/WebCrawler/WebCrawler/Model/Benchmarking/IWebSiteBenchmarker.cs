using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebCrawler.Model.Benchmarking
{
    public interface IWebSiteBenchmarker
    {
        Task<IEnumerable<Entry>> BenchmarkSite(Uri site);
        Task<IEnumerable<Entry>> HistoryOf(Uri uri);
        Task<IEnumerable<Entry>> Mins();
        Task<IEnumerable<Entry>> Maxs();
    }
}