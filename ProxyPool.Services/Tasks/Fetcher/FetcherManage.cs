using ProxyPool.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// 爬取器管理
    /// </summary>
    public static class FetcherManage
    {
        /// <summary>
        /// Web爬取器集合
        /// </summary>
        public static List<IBaseFetcher> Web { get;} = new List<IBaseFetcher>()
        {
            new IP66Fetcher() { Url = "www.66ip.cn" },
            new UuProxyFetcher() { Url = "uu-proxy.com" },
            new KuaidailiFetcher() { Url = "www.kuaidaili.com" },
            new IP3366Fetcher() { Url = "www.ip3366.net" }
        };
    }

    /// <summary>
    /// 基础抓取器
    /// </summary>
    public interface IBaseFetcher
    {
        public string Url { get; set; }
        /// <summary>
        /// 爬取指定网站代理
        /// </summary>
        /// <returns></returns>
        public Task<List<ProxiesFetcherModel>> DoFetcherAsync();
    }

}
