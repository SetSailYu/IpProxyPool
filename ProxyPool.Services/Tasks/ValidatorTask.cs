using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Common;
using ProxyPool.Common.Base;
using ProxyPool.Common.Config;
using ProxyPool.Repository.Base;
using ProxyPool.Services.Models;
using ProxyPool.Services.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly ProxiesService _proxiesService;
        /// <summary>
        /// 验证结果队列
        /// </summary>
        private static ConcurrentQueue<ProxiesQueueModel> _resultQue = new ConcurrentQueue<ProxiesQueueModel>();
        /// <summary>
        /// 删除队列
        /// </summary>
        private static ConcurrentQueue<ProxiesQueueModel> _deleteQue = new ConcurrentQueue<ProxiesQueueModel>();
        /// <summary>
        /// 当前验证记录列表
        /// </summary>
        private static List<Guid> _verifyList = new List<Guid>();
        /// <summary>
        /// 临时记录删除列表
        /// </summary>
        private static List<Guid> _deleteList = new List<Guid>();
        /// <summary>
        /// 验证线程数量配置
        /// </summary>
        private const int VALIDATE_THREAD_NUM = 200;

        public ValidatorTask(IServiceScopeFactory scopeFactory)
        {
            //var scope = scopeFactory.CreateScope();
            //_db = scope.ServiceProvider.GetRequiredService<DB>();
            //每次保存数据前创建新的上下文；这样多线程并发时不会导致异常
            var contextOptions = new DbContextOptionsBuilder<DB>()
                    .UseNpgsql(AppSettingsConfig.ConnectionStrings.PostgreSql).Options;
            _db = new DB(contextOptions);
            _proxiesService = new ProxiesService(_db);
        }

        /// <summary>
        /// 任务体
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override async Task<double> DoTaskAsync(object state)
        {
            ConsoleHelper.WriteHintLog($"【验证器】循环 ====>");
            // --> 检查验证线程是否返回了代理的验证结果
            ConsoleHelper.WriteSuccessLog($"【验证器】_resultQue 1验证结果队列 {_resultQue.Count} ");
            ConsoleHelper.WriteSuccessLog($"【验证器】_verifyList 1当前验证记录列表 {_verifyList.Count} ");
            int out_cnt = 0;
            while (!_resultQue.IsEmpty)
            {
                ProxiesQueueModel model = new ProxiesQueueModel();
                if (_resultQue.TryDequeue(out model))
                {
                    if( await _proxiesService.UpdateValidatorTaskResultAsync(model))
                    {
                        //更新代理
                        _verifyList.Remove(model.Id);//弹出验证记录列表
                        out_cnt++;
                    }  
                }
            }
            ConsoleHelper.WriteSuccessLog($"【验证器】_resultQue 2验证结果队列 {_resultQue.Count} ");
            ConsoleHelper.WriteSuccessLog($"【验证器】_verifyList 2当前验证记录列表 {_verifyList.Count} ");
            ConsoleHelper.WriteSuccessLog($"【验证器】完成了 {out_cnt} 个代理验证");
            ConsoleHelper.WriteSuccessLog($"【验证器】_deleteQue 1删除队列 {_deleteQue.Count} ");
            // --> 删除验证结果指定代理
            while (!_deleteQue.IsEmpty)
            {
                ProxiesQueueModel model = new ProxiesQueueModel();
                if (_deleteQue.TryDequeue(out model)) _deleteList.Add(model.Id);//加入删除记录列表
            }
            ConsoleHelper.WriteSuccessLog($"【验证器】_deleteQue 2删除队列 {_deleteQue.Count} ");
            ConsoleHelper.WriteSuccessLog($"【验证器】_deleteList 1临时记录删除列表 {_deleteList.Count} ");
            if (_deleteList.Count > 0)
            {
                bool success = await _proxiesService.DeleteAsync(_deleteList);  //批量删除
                if (success)
                {
                    _verifyList.RemoveAll(r => _deleteList.Contains(r)); //批量弹出验证记录列表
                    _deleteList.Clear(); //清空临时记录列表
                }
            }
            ConsoleHelper.WriteSuccessLog($"【验证器】_deleteList 2临时记录删除列表 {_deleteList.Count} ");
            // --> 如果正在进行验证的代理足够多，那么就不着急添加新代理
            if (_verifyList.Count >= VALIDATE_THREAD_NUM * 2)
            {
                return 5; //返回5秒间隔循环
            }
            // --> 从数据库中获取若干当前待验证的代理装填进线程池
            int num = 0;
            List<ProxiesQueueModel> proxiesQueues = await _proxiesService.GetProxiesQueueAsync(VALIDATE_THREAD_NUM * 4);
            ConsoleHelper.WriteSuccessLog($"【验证器】proxiesQueues SQL待验证的代理 {proxiesQueues.Count} ");
            foreach (ProxiesQueueModel proxy in proxiesQueues)
            {
                bool join = ThreadPool.QueueUserWorkItem(new WaitCallback(VerifyThreadFun), proxy);
                if (join)
                {
                    _verifyList.Add(proxy.Id); //加入验证记录列表
                    await _proxiesService.UpdateProxyVerifyStateAsync(proxy.Id, 1); //更新为验证中状态
                    num++;  
                }
            }
            ConsoleHelper.WriteHintLog($"【验证器】===========>本轮结束休息5秒，添加了 {num} 个线程，共有 {_verifyList.Count} 个线程在运行");
            return 5; //返回5秒间隔循环
        }

        /// <summary>
        /// 验证线程方法
        /// </summary>
        /// <param name="model"></param>
        public void VerifyThreadFun(object model)
        {
            ProxiesQueueModel item = (ProxiesQueueModel)model;
            ConsoleHelper.WriteSuccessLog($"【验证器】 验证{item.Ip}:{item.Port}中。。。。。。 ");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool success = ValidateProxy(item.Ip, item.Port); //验证代理
            sw.Stop();
            //判断验证结果并加入队列
            ValidateQue(item, success, Convert.ToInt32(sw.Elapsed.TotalMilliseconds));
        }

        /// <summary>
        /// 验证代理
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns>成功true/失败false</returns>
        public bool ValidateProxy(string host, int port)
        {
            string url = $"https://baidu.com";
            string result = Retry.Function(proxy =>
            {
                var res = SpiderBase.GetStreamStr(url, proxy);
                return res;
            }, new RetrySettings 
            {
                MaximumNumberOfAttempts = 3,
                AssignProxyIp = $"{host}:{port}"
            });

            if (!string.IsNullOrEmpty(result) && result.Contains("百度搜索"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断验证结果并加入队列
        /// </summary>
        /// <param name="model"></param>
        /// <param name="success"></param>
        /// <param name="latency"></param>
        /// <returns></returns>
        public void ValidateQue(ProxiesQueueModel model, bool success, int latency)
        {
            if (success)
            {
                model.Success = true;
                model.Latency = latency;
                model.ValidateDate = DateTime.Now.SetKindLocal();
                model.ValidateFailedCnt = 0;
                model.ToValidateDate = DateTime.Now.SetKindLocal().AddMinutes(10); //10分钟之后继续验证
            }
            else
            {
                model.ValidateFailedCnt++;
                if (model.ValidateFailedCnt >= 3)
                {
                    _deleteQue.Enqueue(model); //失败太多次，加入删除队列
                    return;
                }
                model.Success = false;
                model.Latency = latency;
                model.ValidateDate = DateTime.Now.SetKindLocal();
                model.ToValidateDate = DateTime.Now.SetKindLocal().AddMinutes(model.ValidateFailedCnt * 10); //验证失败的次数越多，距离下次验证的时间越长
            }
            // 加入验证结果队列
            _resultQue.Enqueue(model);
        }



    }
}
