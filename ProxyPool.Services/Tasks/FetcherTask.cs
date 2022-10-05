using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Common;
using ProxyPool.Common.Base;
using ProxyPool.Common.Config;
using ProxyPool.Common.Services;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Entity;
using ProxyPool.Services.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProxyPool.Services.Tasks
{
    /// <summary>
    /// 【爬取器】后台任务
    /// </summary>
    public class FetcherTask : InitBackgroundTask
    {
        private DB _db;
        private FetchersService _fetchersService;
        private ProxiesService _proxiesService;

        /// <summary>
        /// 爬取器队列
        /// </summary>
        private static ConcurrentQueue<FetcherQueModel> _fetcherQue = new ConcurrentQueue<FetcherQueModel>();

        public FetcherTask(IServiceScopeFactory scopeFactory)
        {

            //var scope = scopeFactory.CreateScope();
            //_db = scope.ServiceProvider.GetRequiredService<DB>();
            //每次保存数据前创建新的上下文；这样多线程并发时不会导致异常
            //var contextOptions = new DbContextOptionsBuilder<DB>()
            //        .UseNpgsql(AppSettingsConfig.ConnectionStrings.PostgreSql).Options;
            //_db = new DB(contextOptions);
            //_fetchersService = new FetchersService(_db);
            //_proxiesService = new ProxiesService(_db);
        }

        /// <summary>
        /// 任务体
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override async Task<double> DoTaskAsync(object state)
        {
            //每次保存数据前创建新的上下文；这样多线程并发时不会导致异常
            var contextOptions = new DbContextOptionsBuilder<DB>()
                    .UseNpgsql(AppSettingsConfig.ConnectionStrings.PostgreSql).Options;
            _db = new DB(contextOptions);
            _fetchersService = new FetchersService(_db);
            _proxiesService = new ProxiesService(_db);
            //  for 爬取器 in 所有爬取器:
            //    查询数据库，判断当前爬取器是否需要运行
            //    如果需要运行，那么启动线程运行该爬取器
            //  等待所有线程结束
            //  将爬取到的代理放入数据库中
            //  睡眠一段时间
            ConsoleHelper.WriteSuccessLog("【爬取器】循环 ====>");

            ProxiesStatusModel proxiesStatus = await _proxiesService.GetAllProxiesStatusAsync();
            if (proxiesStatus.VerifyCount > 2000)
            {
                ConsoleHelper.WriteHintLog($"【爬取器】还有{proxiesStatus.VerifyCount}个代理等待验证，数量过多，跳过本次爬取");
                return 5 * 60; //爬取器睡眠5分钟 
            }

            foreach (IBaseFetcher fetcher in FetcherManage.Web)
            {
                Fetchers data = await _fetchersService.GetByNameFirstAsync(fetcher.Url);
                if (data == null) continue; //数据库内没有相应信息
                if (!data.Enable) continue; //该爬取器被禁用

                ////加入线程池
                //bool join = ThreadPool.QueueUserWorkItem(new WaitCallback(FetcherThreadFun), new FetcherThreadParamModel
                //{
                //    Data = data,
                //    Fetcher = fetcher
                //});
                if (_fetcherQue.Any(a => a.Url == fetcher.Url))
                {
                    ConsoleHelper.WriteHintLog($"【爬取器】{fetcher.Url} 正在运行中");
                    continue;
                }
                //加入队列运行
                Task fetcherTask = Task.Factory.StartNew(async e =>
                {
                    await FetcherDoTaskAsync(new FetcherDoTaskParamModel
                    {
                        Data = data,
                        Fetcher = fetcher
                    });
                    DeleteFetcherQue(fetcher.Url); //删除记录
                }, new CancellationTokenSource().Token);

                _fetcherQue.Enqueue(new FetcherQueModel() { Url = fetcher.Url, RunTask = fetcherTask });
                ConsoleHelper.WriteHintLog($"【爬取器】{fetcher.Url} 加入线程池成功！");
            }

            return 5 * 60; //爬取器睡眠5分钟
        }

        /// <summary>
        /// 指定爬取器方法
        /// </summary>
        /// <param name="model"></param>
        private async Task<int> FetcherDoTaskAsync(FetcherDoTaskParamModel model)
        {
            //爬取指定网站，获取代理
            List<ProxiesFetcherModel> result = await model.Fetcher.DoFetcherAsync();
            //循环添加代理到数据库
            int count = 0;
            if (result.Count > 0)
            {
                foreach (ProxiesFetcherModel proxy in result)
                {
                    count += await _proxiesService.AddAsync(proxy);
                }
            }
            //更新爬取器状态
            await _fetchersService.UpdateFetcherStateAsync(model.Data.Id, model.Data.SumProxiesCnt + count, count);
            ConsoleHelper.WriteSuccessLog($"【爬取器】{model.Data.Name} 新增 {count} 个代理 <=====");

            return count;
        }

        /// <summary>
        /// 删除队列内指定指定爬取器记录
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public void DeleteFetcherQue(string url)
        {
            var tempQue = Array.Empty<FetcherQueModel>();
            _fetcherQue.CopyTo(tempQue, 0);
            _fetcherQue.Clear();
            foreach (var item in tempQue)
            {
                if (item.Url == url) continue;
                _fetcherQue.Enqueue(item);
            }
            ConsoleHelper.WriteSuccessLog($"【爬取器】删除队列内 {url} 成功 ");
        }

        /// <summary>
        /// 爬取器线程参数Model
        /// </summary>
        public class FetcherDoTaskParamModel
        {
            /// <summary>
            /// 数据库爬取器数据
            /// </summary>
            public Fetchers Data { get; set; }
            /// <summary>
            /// 爬取器实体接口
            /// </summary>
            public IBaseFetcher Fetcher { get; set; }
        }

        public class FetcherQueModel
        {
            /// <summary>
            /// 任务名称
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 正在运行的任务
            /// </summary>
            public Task RunTask { get; set; }
        }




    }
}
