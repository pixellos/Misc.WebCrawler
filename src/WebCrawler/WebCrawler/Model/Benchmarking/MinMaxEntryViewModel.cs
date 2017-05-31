namespace WebCrawler.Model.Benchmarking
{
    public class MinMaxEntryViewModel
    {
        public MinMaxEntryViewModel(string uri, int slowestResponse, int fastestResponse)
        {
            this.Uri = uri;
            this.SlowestResponse = slowestResponse;
            this.FastestResponse = fastestResponse;
        }

        public string Uri { get; }
        public int SlowestResponse { get; }
        public int FastestResponse { get; }
    }
}