using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Models
{
    /// <summary>
    /// 代理抓取Model
    /// </summary>
    public class ProxiesFetcherModel
    {
        /// <summary>
        /// 这个代理来自哪个爬取器 
        /// </summary>
        public string FetcherName { get; set; }
        /// <summary>
        /// 代理协议名称，一般为http/socks
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// 代理的IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 代理的端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 代理的位置
        /// </summary>
        public string Location { get; set; }
    }
}
