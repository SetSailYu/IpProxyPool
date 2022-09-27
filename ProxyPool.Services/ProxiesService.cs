using Microsoft.EntityFrameworkCore;
using ProxyPool.Common;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Entity;
using ProxyPool.Services.Models;

namespace ProxyPool.Services
{
    /// <summary>
    /// 代理服务 
    /// </summary>
    public class ProxiesService
    {
        private readonly DB _db;

        public ProxiesService(DB context)
        {
            _db = context;
        }

        /// <summary>
        /// 获取全部代理状态
        /// </summary>
        /// <returns></returns>
        public ProxiesStatusModel GetAllProxiesStatus()
        {
            ProxiesStatusModel result = new ProxiesStatusModel();
            var res = _db.Set<Proxies>().ToList();
            result.AllCount = res.Count;
            result.PassCount = res.Where(w => w.Validated == true).Count();
            result.VerifyCount = res.Where(w => w.ToValidateDate <= DateTime.Now).Count();
            return result;
        }

        /// <summary>
        /// 获取待验证的代理
        /// </summary>
        /// <param name="maxCount">返回数量限制</param>
        /// <returns></returns>
        public List<ProxiesQueueModel> GetProxiesQueue(int maxCount)
        {
            var query = _db.Set<Proxies>()
                .Where(w => w.ToValidateDate <= DateTime.Now && w.VerifyState == 0)
                .OrderByDescending(o => o.Validated).OrderBy(o => o.ToValidateDate)
                .Select(s => new ProxiesQueueModel
                {
                    Id = s.Id,
                    Ip = s.Ip,
                    Port = s.Port,
                    Success = false,
                    ValidateFailedCnt = s.ValidateFailedCnt,
                    Delete = false
                }).ToList();
            if (maxCount > 0)
            {
                query = query.Take(maxCount).ToList();
            }
            return query;
        }

        /// <summary>
        /// 更新代理的验证状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verifyState">验证状态 （1验证中，0待验证） 默认0</param>
        /// <returns></returns>
        public bool UpdateProxyVerifyState(Guid id, int verifyState)
        {
            var proxy = _db.Set<Proxies>().FirstOrDefault(f => f.Id == id);
            if (proxy == null)
            {
                //ConsoleHelper.WriteErrorLog("找不到代理，更新失败");
                return false;
            }
            proxy.VerifyState = verifyState;
            _db.Update(proxy);
            int ret = _db.SaveChanges();

            if (ret == 0)
            {
                //ConsoleHelper.WriteErrorLog("更新记录数为0");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 获取全部代理列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Proxies>> GetAllAsync()
        {
            var result = await _db.Set<Proxies>().ToListAsync();
            return result;
        }

        /// <summary>
        /// 获取分页代理列表
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<PageResultModel<Proxies>> GetPageAsync(PageRequestModel request)
        {
            var result = new PageResultModel<Proxies>();
            var query = _db.Set<Proxies>().AsQueryable();

            // 根据条件查询
            if (!string.IsNullOrEmpty(request.Protocol))
            {
                query = query.Where(d => EF.Functions.Like(d.Protocol, $"%{request.Protocol}%"));
            }
            if (!string.IsNullOrEmpty(request.FetcherName))
            {
                query = query.Where(d => EF.Functions.Like(d.FetcherName, $"%{request.FetcherName}%"));
            }

            // 获取总数量
            result.TotalRows = await query.CountAsync();

            // 分页查询
            var quer = query.OrderBy(d => d.Protocol).Page(request.PageNo, request.PageSize);
            var res = await query.ToListAsync();
            result.Rows = res;

            result.SetPage(request);
            result.CountTotalPage();

            return result;
        }

    }
}
