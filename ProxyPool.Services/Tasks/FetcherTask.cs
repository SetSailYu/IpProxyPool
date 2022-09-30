using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProxyPool.Common;
using ProxyPool.Common.Base;
using ProxyPool.Common.Services;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Entity;
using ProxyPool.Services.Models;
using System;
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
        private readonly DB _db;
        private readonly FetchersService _fetchersService;
        private readonly ProxiesService _proxiesService;
        public FetcherTask(IServiceScopeFactory scopeFactory)
        {
            var scope = scopeFactory.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<DB>();
            _fetchersService = new FetchersService(_db);
            _proxiesService = new ProxiesService(_db);
        }

        /// <summary>
        /// 任务体
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override async Task<double> DoTaskAsync(object state)
        {
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
                ConsoleHelper.WriteHintLog($"还有{proxiesStatus.VerifyCount}个代理等待验证，数量过多，跳过本次爬取");
                return 5 * 60; //爬取器睡眠5分钟 
            }


            foreach (IBaseFetcher fetcher in FetcherManage.Web)
            {
                Fetchers data = await _fetchersService.GetByNameFirstAsync(fetcher.Url);
                if (data == null) continue; //数据库内没有相应信息
                if (!data.Enable) continue; //该爬取器被禁用

                List<ProxiesFetcherModel> result = await fetcher.DoFetcherAsync();

                // ===== 批量写入代理表

                //更新爬取器状态
                await _fetchersService.UpdateFetcherStateAsync(data.Id, data.SumProxiesCnt + result.Count, result.Count);
            }

            
            



            return 5 * 60; //爬取器睡眠5分钟
        }

        /// <summary>
        /// 爬取器线程参数Model
        /// </summary>
        public class FetcherThreadParamModel
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




    }
}
