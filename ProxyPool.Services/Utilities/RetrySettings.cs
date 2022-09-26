using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    public class RetrySettings
    {
        /// <summary>
        /// 最大重试次数(必须>=1)
        /// </summary>
        public int MaximumNumberOfAttempts { get; set; } = 3;
        /// <summary>
        /// 指定代理IP  127.0.0.1:8080
        /// </summary>
        public WebProxy AssignProxyIp { get; set; }

    }
}
