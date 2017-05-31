using System;

namespace WebCrawler.Helpers
{
    public static class UriExtensions
    {
        public static Uri SkipQuery(this Uri uri)
        {
            var skipped = uri.GetComponents(UriComponents.Host | UriComponents.Scheme | UriComponents.Path, UriFormat.Unescaped);
            var result = new Uri(skipped);
            return result;
        }
    }
}