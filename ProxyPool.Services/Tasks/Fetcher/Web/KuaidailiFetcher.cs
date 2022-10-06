using HtmlAgilityPack;
using ProxyPool.Common;
using ProxyPool.Common.Extensions;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// https://www.kuaidaili.com/free/inha/  高匿
    /// https://www.kuaidaili.com/free/intr/  透明
    /// </summary>
    public class KuaidailiFetcher : IBaseFetcher
    {
        public string Url { get; set; }

        /// <summary>
        /// 主体
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProxiesFetcherModel>> DoFetcherAsync()
        {
            List<ProxiesFetcherModel> result = new List<ProxiesFetcherModel>();
            int maxPage = 50;
            try
            {
                ConsoleHelper.WriteHintLog($"【爬取器】开始爬取 {Url} ====>");
                //批量获取代理
                for (int i = 1; i <= maxPage; i++)   // 高匿
                {
                    var htmlDoc = new HtmlDocument();
                    string html = GetHtml($"https://www.kuaidaili.com/free/inha/{i}");
                    if (string.IsNullOrEmpty(html)) continue;
                    if (!html.Contains("最后验证时间")) continue;
                    html = Regex.Replace(html, @"[\n\r]", "");
                    htmlDoc.LoadHtml(html);//加载html
                    GetAddProxy(htmlDoc, result);
                    Thread.Sleep(1);
                }
                for (int i = 1; i <= maxPage; i++)  // 透明
                {
                    var htmlDoc = new HtmlDocument();
                    string html = GetHtml($"https://www.kuaidaili.com/free/intr/{i}");
                    if (string.IsNullOrEmpty(html)) continue;
                    if (!html.Contains("最后验证时间")) continue;
                    html = Regex.Replace(html, @"[\n\r]", "");
                    htmlDoc.LoadHtml(html);//加载html
                    GetAddProxy(htmlDoc, result);
                    Thread.Sleep(1);
                }
                ConsoleHelper.WriteSuccessLog($"【爬取器】{Url} 本次爬取 {result.Count} 个代理 <=====");
            }
            catch (Exception e)
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
            //获取代理
            var htmlTr = htmlDoc.DocumentNode.SelectNodes("//div[@id = 'list']/table/tbody/tr");
            if (htmlTr != null)
            {
                foreach (var node in htmlTr)
                {
                    HtmlNodeCollection CNodes = node.ChildNodes;
                    result.Add(new ProxiesFetcherModel
                    {
                        FetcherName = this.Url,
                        Protocol = CNodes[7].InnerText.ToLower(),  //默认小写
                        Ip = CNodes[1].InnerText,
                        Port = CNodes[3].InnerText.ToInt32(),
                        Location = CNodes[9].InnerText
                    });
                }
            }
        }

        /// <summary>
        /// html请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHtml(string url)
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
