using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Models
{
    /// <summary>
    /// http://uu-proxy.com/api/free 请求代理结果Model
    /// </summary>
    public class UuProxyModel
    {
        public bool Success { get; set; }

        public FreeData Free { get; set; }

        public class FreeData
        {
            public List<UuProxyData> Proxies { get; set; }
        }
    }

    public class UuProxyData
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Scheme { get; set; }
        public bool SupportHttps { get; set; }
        public string Region { get; set; }
    }
}
