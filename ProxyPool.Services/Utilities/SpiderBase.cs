using Newtonsoft.Json;
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
        /// Get请求
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="proxyIp"></param>
        /// <param name="timeOut"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        /// <exception cref="ProxyTimeoutException"></exception>
        public static string GetStreamStr(AlgorithmDTO dto, WebProxy proxyIp, int timeOut = 6000, bool zip = true)
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
        /// <param name="proxyIp"></param>
        /// <param name="timeOut"></param>
        /// <param name="zip"></param>
        /// <returns></returns>
        public static string GetStr(AlgorithmDTO dto, WebProxy proxyIp, int timeOut = 10000, bool zip = true)
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
                request.Proxy = proxyIp;
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


    }
}
