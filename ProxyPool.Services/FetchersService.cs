using Microsoft.EntityFrameworkCore;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyPool.Services
{
    /// <summary>
    /// 爬取器服务
    /// </summary>
    public class FetchersService
    {
        private readonly DB _db;

        public FetchersService(DB context)
        {
            _db = context;
        }

        /// <summary>
        /// 获取全部代理列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Fetchers>> GetAsync()
        {
            var result = await _db.Set<Fetchers>().ToListAsync();
            return result;
        }

        /// <summary>
        /// 获取分页代理列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PageResultModel<Fetchers>> GetPageAsync(PageRequestModel request)
        {
            var result = new PageResultModel<Fetchers>();
            var query = _db.Set<Fetchers>().AsQueryable();

            // 根据条件查询
            if (!string.IsNullOrEmpty(request.FetcherName))
            {
                query = query.Where(d => EF.Functions.Like(d.Name, $"%{request.FetcherName}%"));
            }
            if (request.Enable.HasValue)
            {
                query = query.Where(d => d.Enable == request.Enable );
            }

            // 获取总数量
            result.TotalRows = await query.CountAsync();

            // 分页查询
            var quer = query.OrderBy(d => d.Enable).Page(request.PageNo, request.PageSize);
            var res = await query.ToListAsync();
            result.Rows = res;

            result.SetPage(request);
            result.CountTotalPage();

            return result;
        }

    }
}
