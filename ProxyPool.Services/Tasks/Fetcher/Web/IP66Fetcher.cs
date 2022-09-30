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

        /// <summary>
        /// 主体
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProxiesFetcherModel>> DoFetcherAsync()
        {
            List<ProxiesFetcherModel> result = new List<ProxiesFetcherModel>();
            int maxPage = 10; 
            try
            {
                string indexHtml = await GetHtml("http://www.66ip.cn/index.html");
                if (string.IsNullOrEmpty(indexHtml)) return result;

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(indexHtml);//加载html
                //获取首页的代理
                GetAddProxy(htmlDoc, result);
                //获取最大页数
                var htmlPage = htmlDoc.DocumentNode.SelectNodes("//div[@id = 'PageList']/a");
                if (htmlPage != null && htmlPage.Count > 1)
                {
                    int page = htmlPage[htmlPage.Count - 2].InnerText.ToInt32();
                    maxPage = page == 0 ? maxPage : page;
                }

                for(int i = 2; i <= maxPage; i++)
                {
                    htmlDoc = new HtmlDocument();
                    string html = await GetHtml($"http://www.66ip.cn/{i}.html");
                    htmlDoc.LoadHtml(html);//加载html
                    GetAddProxy(htmlDoc, result);
                }






            }
            catch(Exception e)
            {
                ConsoleHelper.WriteErrorLog($"Error: {e.Message}");
            }

            return result;
        }

        /// <summary>
        /// 解析并添加代理
        /// </summary>
        /// <param name="result"></param>
        /// <param name="htmlDoc"></param>
        /// <returns></returns>
        private void GetAddProxy(HtmlDocument htmlDoc, List<ProxiesFetcherModel> result)
        {
            //获取首页的代理
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
        }

        /// <summary>
        /// html请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> GetHtml(string url)
        {
            var result = await Retry.Async.FunctionAsync(async proxy =>
            {
                return await SpiderBase.Async.GetHttpHtmlByGB2312Async(url);
            }, new RetrySettings
            {
                MaximumNumberOfAttempts = 3
            });
            return result.Result;
        }

    }
}
