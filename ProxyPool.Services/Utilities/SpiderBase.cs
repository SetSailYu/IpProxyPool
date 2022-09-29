using Newtonsoft.Json;
using ProxyPool.Common.Extensions;
using RandomUserAgent;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    public static class SpiderBase
    {
        /// <summary>
        /// 请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="proxyIp"></param>
        /// <param name="referrer"></param>
        /// <param name="timeOut"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static string GetStreamStr(string url = "", string proxyIp = "", string referrer = "", int timeOut = 6000, bool zip = true)
        {
            AlgorithmDTO dto = new AlgorithmDTO() { Url = url, Referrer = referrer };
            return GetStreamStr(dto, proxyIp, timeOut, zip);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="proxyIp">127.0.0.1:8080</param>
        /// <param name="timeOut"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        /// <exception cref="ProxyTimeoutException"></exception>
        public static string GetStreamStr(AlgorithmDTO dto, string proxyIp = "", int timeOut = 6000, bool zip = true)
        {
            try
            {
                string returnText = GetStr(dto, proxyIp, timeOut, zip);
                return returnText;
            }
            catch (WebException ex)
            {
                return ex.Message;
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="proxyIp">127.0.0.1:8080</param>
        /// <param name="timeOut"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static string GetStr(AlgorithmDTO dto, string proxyIp = "", int timeOut = 10000, bool zip = true)
        {

            var request = (HttpWebRequest)WebRequest.Create(dto.Url);
            request.Method = "GET";
            request.Timeout = timeOut;
            request.ReadWriteTimeout = timeOut;
            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";
            request.KeepAlive = false;

            if (!string.IsNullOrEmpty(dto.UserAgent))
            {
                request.UserAgent = dto.UserAgent;
            }
            else
            {
                request.UserAgent = RandomUa.RandomUserAgent;
            }


            if (!string.IsNullOrEmpty(dto.Referrer))
            {
                request.Referer = dto.Referrer;
            }

            if (!string.IsNullOrEmpty(dto.RequestCookie))
            {
                request.Headers["Cookie"] = dto.RequestCookie;
            }

            if (proxyIp != null)
            {
                request.Proxy = proxyIp.GetProxy();
            }

            if (dto.ExtraHeaders != null)
            {
                foreach (var item in dto.ExtraHeaders)
                {
                    request.Headers[item.Key] = item.Value;
                }
            }
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var resStr = "";
            using (var response = (HttpWebResponse)request.GetResponse())
            {

                using (Stream responseStream = response.GetResponseStream())
                {
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        using (GZipStream stream = new GZipStream(responseStream, CompressionMode.Decompress))
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                resStr = reader.ReadToEnd();
                            }
                        }
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using (DeflateStream stream = new DeflateStream(responseStream, CompressionMode.Decompress))
                        {
                            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                resStr = reader.ReadToEnd();
                            }
                        }
                    }
                    else
                    {
                        using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            resStr = myStreamReader.ReadToEnd();
                        }
                    }

                }
            }
            if (request != null)
            {
                request.Abort();
            }
            return resStr;
        }

        /// <summary>
        /// 对代理ip进行处理
        /// </summary>
        /// <param name="proxyIp">代理ip</param>
        /// <returns></returns>
        public static WebProxy GetProxy(this string proxyIp)
        {
            WebProxy proxy = null;
            if (!string.IsNullOrEmpty(proxyIp))
            {
                string[] arr = proxyIp.Split(':');
                if (arr.Length > 0)
                {
                    string ip = arr[0];
                    int port = 80;
                    if (arr.Length > 1)
                    {
                        port = arr[1].ToInt32();
                    }
                    proxy = new WebProxy(ip, port);
                    if (arr.Length == 4) proxy.Credentials = new NetworkCredential(arr[2], arr[3]);
                }
            }
            return proxy;
        }


    }
}
