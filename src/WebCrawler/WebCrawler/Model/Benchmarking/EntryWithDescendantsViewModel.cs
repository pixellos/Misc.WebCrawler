using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler.Model.Benchmarking
{
    public class EntryWithDescendantsViewModel :EntryViewModel
    {
        public EntryWithDescendantsViewModel(string uri, int miliSeconds, IEnumerable<string> descendants) : base(uri, miliSeconds)
        {
            this.Descentants = descendants;
        }

        public IEnumerable<string> Descentants { get; }
    }
}
