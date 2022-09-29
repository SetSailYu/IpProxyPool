using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Common.Base
{
    /// <summary>
    /// BackgroundService拓展
    /// </summary>
    public abstract class InitBackgroundTask : BackgroundService
    {
        /// <summary>
        /// 创建一个取消标记源
        /// </summary>
        public readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 入参委托
        /// </summary>
        private Action<object> action;

        /// <summary>
        /// 方法初始化
        /// </summary>
        protected void Init()
        {
            action = async e =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(await DoTaskAsync(e))); //异步延迟
                }
            };
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Init();
            _ = Task.Factory.StartNew(action, cancellationTokenSource.Token);

            ConsoleHelper.WriteSuccessLog($"--后台任务开启----->{cancellationTokenSource.Token}");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 关闭方法
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            cancellationTokenSource.Cancel();
            return base.StopAsync(cancellationToken);
        }

        /// <summary>
        /// 抛出方法入口
        /// </summary>
        /// <param name="db"></param>
        /// <returns>异步延迟间隔（秒）</returns>
        protected abstract Task<double> DoTaskAsync(object state);
    }
}
