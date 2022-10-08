using HtmlAgilityPack;
using Newtonsoft.Json;
using ProxyPool.Common;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks.Fetcher.Web
{
    /// <summary>
    /// https://ip.jiangxianli.com/
    /// </summary>
    public class JiangxianliFetcher : IBaseFetcher
    {
        public string Url { get; set; }

        public async Task<List<ProxiesFetcherModel>> DoFetcherAsync()
        {
            List<ProxiesFetcherModel> result = new List<ProxiesFetcherModel>();
            int maxPage = 100;
            try
            {
                ConsoleHelper.WriteHintLog($"【爬取器】开始爬取 {Url} ====>");
                string strData = Spider.GetData("https://ip.jiangxianli.com/api/proxy_ips");
                if (string.IsNullOrEmpty(strData)) return result;

                JiangxianliModel model = JsonConvert.DeserializeObject<JiangxianliModel>(strData);
                if (model == null) return result;
                if (model.Msg == "成功" && model.Data != null)
                {
                    foreach (var item in model.Data.Data)
                    {
                        result.Add(new ProxiesFetcherModel
                        {
                            FetcherName = this.Url,
                            Protocol = item.Protocol,
                            Ip = item.Ip,
                            Port = item.Port,
                            Location = item.IpAddress
                        });
                    }
                }
                ConsoleHelper.WriteSuccessLog($"【爬取器】{Url} 本次爬取 {result.Count} 个代理 <=====");
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteErrorLog($"Error: {e.Message}");
            }

            return result;
        }

    }
}
