using ProxyPool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    public static class Retry
    {
        /// <summary>
        /// 重试机制（RetrySettings里面传值）
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="fun">传进的委托（方法）</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Function<T>(Func<WebProxy, T> fun, RetrySettings settings) where T : class
        {
            var numberOfAttempts = 0;
            //循环settings里面的MaximumNumberOfAttempts次数
            while (numberOfAttempts < settings.MaximumNumberOfAttempts)
            {
                try
                {
                    var result = fun(settings.AssignProxyIp);
                    return result;
                }
                catch (Exception ex)
                {
                    numberOfAttempts++;
                }
            }

            return null;
        }

    }
}
