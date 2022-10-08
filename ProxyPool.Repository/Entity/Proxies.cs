using ProxyPool.Repository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Repository.Entity
{
    /// <summary>
    /// 代理  
    /// </summary>
    public class Proxies : EntityBase<Guid>
    {
        /// <summary>
        /// 这个代理来自哪个爬取器 
        /// </summary>
        [Column("fetcher_name")]
        public string FetcherName { get; set; }
        /// <summary>
        /// 代理协议名称，一般为http/socks
        /// </summary>
        [Column("protocol")]
        public string Protocol { get; set; }
        /// <summary>
        /// 代理的IP地址
        /// </summary>
        [Column("ip")]
        public string Ip { get; set; }
        /// <summary>
        /// 代理的端口号
        /// </summary>
        [Column("port")]
        public int Port { get; set; }
        /// <summary>
        /// 代理的位置
        /// </summary>
        [Column("location")]
        public string? Location { get; set; }
        /// <summary>
        /// 这个代理是否通过了验证，通过了验证表示当前代理可用
        /// </summary>
        [Column("validated")]
        public bool Validated { get; set; }
        /// <summary>
        /// 延迟(单位毫秒)，表示上次验证所用的时间，越小则代理质量越好
        /// </summary>
        [Column("latency")]
        public int Latency { get; set; } = -1;
        /// <summary>
        /// 上一次进行验证的时间（Local无时区）
        /// </summary>
        [Column("validate_date")]
        public DateTime? ValidateDate { get; set; }
        /// <summary>
        /// 下一次进行验证的时间（Local无时区）
        /// </summary>
        [Column("to_validate_date")]
        public DateTime ToValidateDate { get; set; }
        /// <summary>
        /// 已经连续验证失败了多少次，会影响下一次验证的时间
        /// </summary>
        [Column("validate_failed_cnt")]
        public int ValidateFailedCnt { get; set; }
        /// <summary>
        /// 验证状态 （1验证中，0待验证） 默认0
        /// </summary>
        [Column("verify_state")]
        public int VerifyState { get; set; }
    }
}
