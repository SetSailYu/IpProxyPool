using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using ProxyPool.Common;
using ProxyPool.Common.Extensions;
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
    /// <summary>
    /// www.66ip.cn
    /// </summary>
    public class IP66Fetcher : IBaseFetcher
    {
        public string Url { get; set; }

        public async Task<List<ProxiesFetcherModel>> DoFetcherAsync()
        {
            List<ProxiesFetcherModel> result = new List<ProxiesFetcherModel>();
            int maxPage = 10; 
            try
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
                if (string.IsNullOrEmpty(indexHtml)) return result;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(indexHtml);//加载html

                var htmlTr = htmlDoc.DocumentNode.SelectNodes("//div[@id = 'main']/div/div/div/table/tr");
                foreach (var node in htmlTr)
                {
                    if (node.InnerText.Contains("ip")) continue;
                    HtmlNodeCollection CNodes = node.ChildNodes;
                    result.Add(new ProxiesFetcherModel
                    {
                        FetcherName = this.Url,
                        Protocol = "http",
                        Ip = CNodes[0].InnerText,
                        Port = CNodes[1].InnerText.ToInt32(),
                        Location = CNodes[2].InnerText
                    });
                }

                //获取最大页数
                var htmlPage = htmlDoc.DocumentNode.SelectNodes("//div[@id = 'PageList']/a");
                if (htmlPage != null && htmlPage.Count > 1)
                {
                    int page = htmlPage[htmlPage.Count - 2].InnerText.ToInt32();
                    maxPage = page == 0 ? maxPage : page;
                }

            }
            catch(Exception e)
            {
                ConsoleHelper.WriteErrorLog($"Error: {e.Message}");
            }

            return result;
        }
    }
}
