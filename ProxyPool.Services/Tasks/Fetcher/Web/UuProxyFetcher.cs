using Newtonsoft.Json;
using ProxyPool.Common;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// http://uu-proxy.com/api/free
    /// </summary>
    public class UuProxyFetcher : IBaseFetcher
    {
        public string Url { get; set; }

        /// <summary>
        /// 主体
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProxiesFetcherModel>> DoFetcherAsync()
        {
            List<ProxiesFetcherModel> result = new List<ProxiesFetcherModel>();
            try
            {
                ConsoleHelper.WriteHintLog($"【爬取器】开始爬取 {Url} ====>");
                string strData = GetData("http://uu-proxy.com/api/free");
                if (string.IsNullOrEmpty(strData)) return result;

                UuProxyModel model = JsonConvert.DeserializeObject<UuProxyModel>(strData);
                if (model == null) return result;
                if (model.Success && model.Free != null)
                {
                    foreach(var item in model.Free.Proxies)
                    {
                        result.Add(new ProxiesFetcherModel
                        {
                            FetcherName = this.Url,
                            Protocol = item.Scheme,
                            Ip = item.Ip,
                            Port = item.Port,
                            Location = item.Region
                        });
                    }
                }
                //批量获取代理
                
                ConsoleHelper.WriteSuccessLog($"【爬取器】{Url} 本次爬取 {result.Count} 个代理 <=====");
            }
            catch (Exception e)
            {
                ConsoleHelper.WriteErrorLog($"Error: {e.Message}");
            }

            return result;
        }

        /// <summary>
        /// 数据请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetData(string url)
        {
            return Retry.Function( proxy =>
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
