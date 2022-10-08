using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Models
{
    /// <summary>
    /// https://ip.jiangxianli.com/api/proxy_ips
    /// </summary>
    public class JiangxianliModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public JiangxianliData Data { get; set; }
    }

    public class JiangxianliData
    {
        public List<JiangxianliDataInfo> Data { get; set; }
    }

    public class JiangxianliDataInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Country { get; set; }
        public string IpAddress { get; set; }
        public string Protocol { get; set; }
    }

}
