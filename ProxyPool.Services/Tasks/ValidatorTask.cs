using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Common;
using ProxyPool.Common.Base;
using ProxyPool.Repository.Base;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// 【验证器】后台任务
    /// </summary>
    public class ValidatorTask : InitBackgroundTask
    {
        private readonly DB _db;
        private readonly FetchersService _fetchersService;
        private readonly ProxiesService _proxiesService;
        /// <summary>
        /// 验证结果队列
        /// </summary>
        private static ConcurrentQueue<ProxiesQueueModel> _resultQue = new ConcurrentQueue<ProxiesQueueModel>();
        static int cnt = 10;
        static AutoResetEvent myEvent = new AutoResetEvent(false);

        public ValidatorTask(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<DB>();
            _fetchersService = new FetchersService(_db);
            _proxiesService = new ProxiesService(_db);
            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(100, 100);
        }
        protected override double DoTask(object state)
        {
            //  从数据库中获取若干当前待验证的代理
            //  将代理发送给前面创建的线程
            ConsoleHelper.WriteSuccessLog("【验证器】循环 ====>");

            //  检查验证线程是否返回了代理的验证结果
            int out_cnt = 0;
            while (!_resultQue.IsEmpty)
            {
                ProxiesQueueModel model = new ProxiesQueueModel();
                if (_resultQue.TryDequeue(out model))
                {
                    ConsoleHelper.WriteSuccessLog($"弹出结果队列：{model.Ip}");
                    out_cnt++;
                }
            }
            ConsoleHelper.WriteHintLog($"完成了 {out_cnt} 个代理验证");

            //  如果正在进行验证的代理足够多，那么就不着急添加新代理
            //if ()
            //{

            //}

            //for (int i = 1; i <= cycleNum; i++)
            //{
            //    ThreadPool.QueueUserWorkItem(new WaitCallback(testFun), i.ToString());
            //}
            Console.WriteLine("主线程执行！");
            Console.WriteLine("主线程结束！");
            myEvent.WaitOne();
            Console.WriteLine("线程池终止！");
            Console.ReadKey();

            return 2; //返回2秒间隔循环
        }

        public static void testFun(object obj)
        {
            cnt -= 1;
            Console.WriteLine(string.Format("{0}:第{1}个线程", DateTime.Now.ToString(), obj.ToString()));
            Thread.Sleep(5000);
            if (cnt == 0)
            {
                myEvent.Set();
            }
        }

        /// <summary>
        /// 验证代理
        /// </summary>
        /// <returns></returns>
        public bool ValidateProxy(string host, int port)
        {
            RetrySettings settings = new RetrySettings()
            {
                MaximumNumberOfAttempts = 3,
                AssignProxyIp = new WebProxy(host, port)
            };
            AlgorithmDTO dto = new AlgorithmDTO()
            {
                Url = $"https://baidu.com",
            };
            string result = Retry.Function(proxy =>
            {
                var res = SpiderBase.GetStreamStr(dto, proxy);
                return res;
            }, settings);

            if (!string.IsNullOrEmpty(result) && result.Contains("百度搜索"))
            {
                return true;
            }
            return false;
        }

    }
}
