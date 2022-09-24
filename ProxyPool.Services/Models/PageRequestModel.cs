using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services
{
    public class IdRequestModel
    {
        public Guid Id { get; set; }
    }

    public class PageRequestModel
    {
        /// <summary>
        /// 排序字段，暂时无效（!Problem）
        /// </summary>
        public virtual string Sort { get; set; } = "Sort";

        /// <summary>
        /// 页码，从1开始
        /// </summary>
        public virtual int PageNo { get; set; } = 1;

        /// <summary>
        /// 每页记录数
        /// </summary>
        public virtual int PageSize { get; set; } = 10;

        /// <summary>
        /// 代理协议名称，一般为http/socks
        /// </summary>
        public virtual string? Protocol { get; set; }

        /// <summary>
        /// 爬取器名称
        /// </summary>
        public virtual string? FetcherName { get; set; }

        /// <summary>
        /// 是否起用
        /// </summary>
        public virtual bool? Enable { get; set; }
    }
}
