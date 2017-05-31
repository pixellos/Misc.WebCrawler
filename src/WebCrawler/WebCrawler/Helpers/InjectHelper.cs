using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCrawler.Helpers
{
    public static class InjectHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tin"></param>
        /// <returns></returns>
        public static TOut InjectTo<TOut>(this object input)
        {
            var tOut = Activator.CreateInstance<TOut>();
            Omu.ValueInjecter.StaticValueInjecter.InjectFrom(tOut, input);
            return tOut;
        }
    }
}
