namespace WebCrawler.Model.Benchmarking
{
    public class EntryViewModel
    {
        public EntryViewModel(string uri, int miliSeconds)
        {
            this.Uri = uri;
            this.Miliseconds = miliSeconds;
        }

        public string Uri { get; }
        public int Miliseconds { get; }
    }
}