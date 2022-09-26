using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Models
{
    /// <summary>
    /// 代理状态
    /// </summary>
    public class ProxiesStatusModel
    {
        /// <summary>
        /// 全部代理数量
        /// </summary>
        public int AllCount { get; set; }
        /// <summary>
        /// 当前可用代理数量
        /// </summary>
        public int PassCount { get; set; }
        /// <summary>
        /// 等待验证代理数量
        /// </summary>
        public int VerifyCount { get; set; }
    }
}
