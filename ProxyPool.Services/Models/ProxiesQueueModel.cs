using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Models
{
    /// <summary>
    /// 代理验证队列模型
    /// </summary>
    public class ProxiesQueueModel
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 代理的IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 代理的端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 验证是否成功 (True/False) 默认false
        /// </summary>
        public bool Success { get; set; } = false;
        /// <summary>
        /// 本次验证所用的时间(单位毫秒)  默认-1
        /// </summary>
        public int Latency { get; set; } = -1;
        /// <summary>
        /// 已经连续验证失败了多少次，会影响下一次验证的时间
        /// </summary>
        public int ValidateFailedCnt { get; set; }
        /// <summary>
        /// 本次进行验证的时间
        /// </summary>
        public DateTime? ValidateDate { get; set; }
        /// <summary>
        /// 下一次进行验证的时间
        /// </summary>
        public DateTime ToValidateDate { get; set; }
        /// <summary>
        /// 删除 (默认false)
        /// </summary>
        public bool Delete { get; set; } = false;
    }
}
