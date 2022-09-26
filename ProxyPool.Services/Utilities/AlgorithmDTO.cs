using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    public class AlgorithmDTO
    {
        /// <summary>
        /// 请求的Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求的UA
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 请求的Cookie
        /// </summary>
        public string RequestCookie { get; set; }

        public string SessData { get; set; }

        public string Referrer { get; set; }

        public Dictionary<string, string> ExtraHeaders { get; set; }
    }
}
