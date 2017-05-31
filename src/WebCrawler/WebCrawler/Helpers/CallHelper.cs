using System;
using System.Threading.Tasks;

namespace WebCrawler.Helpers
{
    public static class CallHelper
    {
        public async static Task<T> OrberCall<T>(this Task<T> t, Action before, Action after)
        {
            before();
            await t;
            t.Wait();
            var result = await await t.ContinueWith(x =>
            {
                after();
                return x;
            });
            return result;
        }
    }
}