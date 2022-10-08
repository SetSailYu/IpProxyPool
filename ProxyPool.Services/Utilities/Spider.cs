using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    /// <summary>
    /// 网络
    /// </summary>
    public static class Spider
    {
        /// <summary>
        /// 数据请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetData(string url)
        {
            return Retry.Function(proxy =>
            {
                var res = SpiderBase.GetStreamStr(url);
                return res;
            }, new RetrySettings
            {
                MaximumNumberOfAttempts = 3
            });
        }
    }
}
