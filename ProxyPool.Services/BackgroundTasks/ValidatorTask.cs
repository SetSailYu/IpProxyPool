using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Common;
using ProxyPool.Common.Base;
using ProxyPool.Repository.Base;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.BackgroundTasks
{
    /// <summary>
    /// 【验证器】后台任务
    /// </summary>
    public class ValidatorTask : InitBackgroundTask
    {
        private readonly DB _db;
        private readonly FetchersService _fetchersService;
        private readonly ProxiesService _proxiesService;
        public ValidatorTask(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<DB>();
            _fetchersService = new FetchersService(_db);
            _proxiesService = new ProxiesService(_db);
        }
        protected override double DoTask(object state)
        {
            //  检查验证线程是否返回了代理的验证结果
            //  从数据库中获取若干当前待验证的代理
            //  将代理发送给前面创建的线程
            ConsoleHelper.WriteSuccessLog("【验证器】循环 ====>");



            return 2; //返回2秒间隔循环
        }

        public bool ValidateProxy()
        {
            RetrySettings settings = new RetrySettings()
            {
                MaximumNumberOfAttempts = 3,
                AssignProxyIp = new WebProxy()
            };
            AlgorithmDTO dto = new AlgorithmDTO()
            {
                Url = $"",
            };
            string result = Retry.Function(proxy =>
            {
                var res = SpiderBase.GetStreamStr(dto, proxy);
                return res;

            }, settings);

            return true;
        }

    }
}
