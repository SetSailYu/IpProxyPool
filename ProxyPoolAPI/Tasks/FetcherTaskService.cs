using ProxyPool.Common;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Entity;
using ProxyPool.Services;

namespace ProxyPoolAPI.Tasks
{
    public class FetcherTaskService : ITaskService
    {
        private readonly DB _db;

        public FetcherTaskService(DB context)
        {
            _db = context;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Execute()
        {
            ConsoleHelper.WriteHintLog($"--》测试开始 ");
            
            Sun();
        }

        public void Sun()
        {
            int i = 0;
            FetchersService service = new FetchersService(_db);
            while (true)
            {

                //List<string> flower = new List<string>
                //{
                //    "梅花", "水仙", "樱花", "郁金香", "兰花", "桃花", "油菜花", "李花", "蓝莓花", "鸽子花", "梨花", "羊蹄甲", "鲁冰花",
                //    "杜鹃花", "刺梨花", "牡丹花", "薰衣草", "马鞭草", "荷花", "紫薇花", "油葵花", "格桑花", "乔麦花", "韭菜花", "彼岸花",
                //    "桂花", "菊花", "油茶花", "三角梅", "山茶花", "雪松", "长寿花", "腊梅"
                //};

                try
                {
                    
                    ConsoleHelper.WriteHintLog($"--》测试 ==》 {i++}");

                    Fetchers fetchers = service.GetFirst();

                    if (fetchers == null)
                    {
                        ConsoleHelper.WriteErrorLog("返回错误！！！");
                    }else
                    {
                        ConsoleHelper.WriteSuccessLog($"返回成功 ==》 {fetchers.Name}");
                    }

                }
                catch (Exception e)
                {
                    ConsoleHelper.WriteErrorLog(" ERROR -> " + e.Message);
                }

                Thread.Sleep(1500);
                //Console.WriteLine();

                //ConsoleHelper.WriteHintLog("是否结束任务？ y/n ");
                //string end = Console.ReadLine();
                //if (end.Equals("y") || end.Equals("Y"))
                //{
                //    ConsoleHelper.WriteHintLog(" -->  Task End!  <-- ");
                //    return;
                //}

            }
        }

    }
}
