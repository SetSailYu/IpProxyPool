using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Common;
using ProxyPool.Common.Base;
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
        private readonly FetchersService _fetchersService;
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
            var scope = scopeFactory.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<DB>();
            _fetchersService = new FetchersService(_db);
            _proxiesService = new ProxiesService(_db);
            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(100, 100); //最高并发数
        }
        protected override double DoTask(object state)
        {
            ConsoleHelper.WriteSuccessLog($"【验证器】循环 ====>");

            // --> 检查验证线程是否返回了代理的验证结果
            int out_cnt = 0;
            while (!_resultQue.IsEmpty)
            {
                ProxiesQueueModel model = new ProxiesQueueModel();
                if (_resultQue.TryDequeue(out model))
                {
                    _proxiesService.UpdateValidatorTaskResult(model);  //更新代理
                    _verifyList.Remove(model.Id);//弹出验证记录列表
                    out_cnt++;
                }
            }
            ConsoleHelper.WriteSuccessLog($"完成了 {out_cnt} 个代理验证");
            // --> 删除验证结果指定代理
            while (!_deleteQue.IsEmpty)
            {
                ProxiesQueueModel model = new ProxiesQueueModel();
                if (_deleteQue.TryDequeue(out model)) _deleteList.Add(model.Id);//加入删除记录列表
            }
            if (_deleteList.Count > 0)
            {
                ConsoleHelper.WriteErrorLog($"开始批量删除 {_deleteList.Count} 个代理");
                bool success = _proxiesService.Delete(_deleteList);  //批量删除
                if (success)
                {
                    _verifyList.RemoveAll(r => _deleteList.Contains(r)); //批量弹出验证记录列表
                    _deleteList.Clear(); //清空临时记录列表
                }
            }
            

            // --> 如果正在进行验证的代理足够多，那么就不着急添加新代理
            if (_verifyList.Count >= VALIDATE_THREAD_NUM * 2)
            {
                ConsoleHelper.WriteHintLog($"====> 代理足够多 ===========>本轮结束休息5秒<=============");
                return 5; //返回5秒间隔循环
            }

            // --> 从数据库中获取若干当前待验证的代理装填进线程池
            int num = 0;
            List<ProxiesQueueModel> proxiesQueues = _proxiesService.GetProxiesQueue(VALIDATE_THREAD_NUM * 4);
            ConsoleHelper.WriteSuccessLog($"======> 从数据库获取了 {proxiesQueues.Count} 个代理");
            foreach (ProxiesQueueModel proxy in proxiesQueues)
            {
                bool join = ThreadPool.QueueUserWorkItem(new WaitCallback(VerifyThreadFun), proxy);
                if (join)
                {
                    _verifyList.Add(proxy.Id); //加入验证记录列表
                    _proxiesService.UpdateProxyVerifyState(proxy.Id, 1);
                    num++;  
                }
            }
            ConsoleHelper.WriteSuccessLog($"=====> 本轮添加了 {num} 个线程，共有 {_verifyList.Count} 个线程在运行");
            ConsoleHelper.WriteHintLog($"===========>本轮结束休息5秒<=============");
            return 5; //返回5秒间隔循环
        }

        /// <summary>
        /// 验证线程方法
        /// </summary>
        /// <param name="model"></param>
        public void VerifyThreadFun(object model)
        {
            ProxiesQueueModel item = (ProxiesQueueModel)model;
            ConsoleHelper.WriteSuccessLog($"==> 验证 {item.Ip}:{item.Port} 线程");
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
                ConsoleHelper.WriteSuccessLog($"【验证】{host}:{port} 成功 ====>");
                return true;
            }
            ConsoleHelper.WriteErrorLog($"【验证】{host}:{port} 失败 ====>");
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
                model.ValidateDate = DateTimeNow.Local;
                model.ValidateFailedCnt = 0;
                model.ToValidateDate = DateTimeNow.Local.AddMinutes(10); //10分钟之后继续验证
            }
            else
            {
                model.ValidateFailedCnt++;
                if (model.ValidateFailedCnt >= 6)
                {
                    _deleteQue.Enqueue(model); //失败太多次，加入删除队列
                    return;
                }
                model.Success = false;
                model.Latency = latency;
                model.ValidateDate = DateTimeNow.Local;
                model.ToValidateDate = DateTimeNow.Local.AddMinutes(model.ValidateFailedCnt * 10); //验证失败的次数越多，距离下次验证的时间越长
            }
            // 加入验证结果队列
            _resultQue.Enqueue(model);
        }



    }
}
