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
    /// 爬取器
    /// </summary>
    public class Fetchers : EntityBase<Guid>
    {
        /// <summary>
        /// 爬取器的名称
        /// </summary>
        [Column("name")]
        public string Name { get; set; }
        /// <summary>
        /// 是否启用这个爬取器，被禁用的爬取器不会在之后被运行，但是其之前爬取的代理依然存在
        /// </summary>
        [Column("enable")]
        public bool Enable { get; set; }
        /// <summary>
        /// 至今为止总共爬取到了多少个代理
        /// </summary>
        [Column("sum_proxies_cnt")]
        public int SumProxiesCnt { get; set; }
        /// <summary>
        /// 上次爬取到了多少个代理
        /// </summary>
        [Column("last_proxies_cnt")]
        public int LastProxiesCnt { get; set; }
        /// <summary>
        /// 上次爬取的时间
        /// </summary>
        [Column("last_fetch_date")]
        public DateTime? LastFetchDate { get; set; }
    }
}
