using ProxyPool.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Utilities
{
    /// <summary>
    /// 轮询操作
    /// </summary>
    public static class Retry
    {
        /// <summary>
        /// 异步
        /// </summary>
        public static RetryAsync Async = new RetryAsync();

        /// <summary>
        /// 同步重试机制（RetrySettings里面传值）
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="fun">传进的委托（方法）</param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static T Function<T>(Func<string, T> fun, RetrySettings settings) where T : class
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

    /// <summary>
    /// 异步轮询操作
    /// </summary>
    public class RetryAsync
    {
        /// <summary>
        /// 异步重试机制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fun"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<T> FunctionAsync<T>(Func<string, T> fun, RetrySettings settings) where T : class
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
