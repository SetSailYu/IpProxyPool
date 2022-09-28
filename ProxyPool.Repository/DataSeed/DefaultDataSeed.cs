using Microsoft.EntityFrameworkCore;
using ProxyPool.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Repository.DataSeed
{
    public class DefaultDataSeed : IConfigureDataSeed
    {
        /// <summary>
        /// 配置种子数据 
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureDataSeed(ModelBuilder builder)
        {
            // Fetchers  爬取器
            builder.Entity<Fetchers>().HasData(new Fetchers[]
            {
                new Fetchers()
                {
                    Id = new Guid("08da7df0-d44a-4332-8a5c-7921cdc25450"),
                    Name = "测试1",
                    Enable = true,
                    SumProxiesCnt = 1,
                    LastProxiesCnt = 1,
                    LastFetchDate = DateTimeNow.Local
                },
                new Fetchers()
                {
                    Id = new Guid("08da7e1c-95ee-4ba2-8226-ff1d286558bb"),
                    Name = "测试2",
                    Enable = true,
                    SumProxiesCnt = 0,
                    LastProxiesCnt = 0,
                    LastFetchDate = DateTimeNow.Local
                },
                new Fetchers()
                {
                    Id = new Guid("08da7e1c-95ee-4ba2-8226-ff1d286558aa"),
                    Name = "测试3",
                    Enable = false,
                    SumProxiesCnt = 3,
                    LastProxiesCnt = 3,
                    LastFetchDate = DateTimeNow.Local
                },
            });

            // Proxies  代理
            builder.Entity<Proxies>().HasData(new Proxies[]
            {
                new Proxies()
                {
                    Id = new Guid("08da7eb0-c4a6-4bd5-81da-9698273a18ea"),
                    FetcherName = "测试1",
                    Protocol = "http",
                    Ip = "127.0.0.1",
                    Port = 3030,
                    Location = "福州",
                    Validated = true,
                    Latency = 234,
                    ValidateDate = DateTimeNow.Local,
                    ToValidateDate = DateTimeNow.Local.AddMinutes(5),
                    ValidateFailedCnt = 3,
                },
            });
        }
    }
}
