using Microsoft.Extensions.Hosting;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    public class IP66Fetcher : BaseFetcher
    {
        protected override async Task<ProxiesFetcherModel> DoFetcherAsync()
        {
            string url = $"http://www.66ip.cn/index.html";
            string indexHtml = Retry.Function(proxy =>
            {
                var res = SpiderBase.GetStreamStr(url);
                return res;
            }, new RetrySettings()
            {
                MaximumNumberOfAttempts = 3
            });

            throw new NotImplementedException();
        }
    }
}
