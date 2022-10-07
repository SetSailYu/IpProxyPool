using Microsoft.EntityFrameworkCore;
using ProxyPool.Common;
using ProxyPool.Common.Components.DependencyInjection;
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
        /// 随机获取一个代理
        /// </summary>
        /// <returns></returns>
        public async Task<Proxies> GetRandom()
        {
            var query = await _db.Set<Proxies>().Where(w => w.Validated == true)
                .ToListAsync();
            var res = query.OrderBy(o => Guid.NewGuid()).FirstOrDefault();
            return res;
        }

        /// <summary>
        /// 随机获取一个http协议代理
        /// </summary>
        /// <returns></returns>
        public async Task<Proxies> GetRandomHttp()
        {
            var query = await _db.Set<Proxies>().Where(w => w.Validated == true && w.Protocol.Contains("http"))
                .ToListAsync();
            var res = query.OrderBy(o => Guid.NewGuid()).FirstOrDefault();
            return res;
        }

        /// <summary>
        /// 获取全部代理状态
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAllTrueSum()
        {
            var res = await _db.Set<Proxies>().Where(w => w.Validated == true).ToListAsync();
            return res.Count;
        }

        /// <summary>
        /// 获取全部代理状态
        /// </summary>
        /// <returns></returns>
        public async Task<ProxiesStatusModel> GetAllProxiesStatusAsync()
        {
            ProxiesStatusModel result = new ProxiesStatusModel();
            var res = await _db.Set<Proxies>().ToListAsync();
            result.AllCount = res.Count;
            result.PassCount = res.Where(w => w.Validated == true).Count();
            DateTime time = DateTime.Now.SetKindLocal();
            result.VerifyCount = res.Where(w => w.ToValidateDate <= time).Count();
            return result;
        }

        #region 【验证器】

        /// <summary>
        /// 【验证器】获取待验证的代理
        /// </summary>
        /// <param name="maxCount">返回数量限制 (-1全部)</param>
        /// <returns></returns>
        public async Task<List<ProxiesQueueModel>> GetProxiesQueueAsync(int maxCount)
        {
            DateTime time = DateTime.Now.SetKindLocal();
            if (maxCount < 0)
            {
                return await _db.Set<Proxies>()
                    .Where(w => w.ToValidateDate <= time && w.VerifyState == 0)
                    .OrderBy(o => o.ToValidateDate)
                    .Select(s => new ProxiesQueueModel
                    {
                        Id = s.Id,
                        Ip = s.Ip,
                        Port = s.Port,
                        Success = false,
                        ValidateFailedCnt = s.ValidateFailedCnt
                    })
                    .ToListAsync();
            }
            var query = await _db.Set<Proxies>()
                .Where(w => w.ToValidateDate <= time && w.VerifyState == 0 && w.Validated == true)
                .OrderBy(o => o.ToValidateDate)
                .Select(s => new ProxiesQueueModel
                {
                    Id = s.Id,
                    Ip = s.Ip,
                    Port = s.Port,
                    Success = false,
                    ValidateFailedCnt = s.ValidateFailedCnt
                })
                .Take(maxCount)
                .ToListAsync();
            if (query.Count < maxCount)
            {
                int fCount = maxCount - query.Count;
                var queryF = await _db.Set<Proxies>()
                    .Where(w => w.ToValidateDate <= time && w.VerifyState == 0 && w.Validated == false)
                    .OrderBy(o => o.ToValidateDate)
                    .Select(s => new ProxiesQueueModel
                    {
                        Id = s.Id,
                        Ip = s.Ip,
                        Port = s.Port,
                        Success = false,
                        ValidateFailedCnt = s.ValidateFailedCnt
                    })
                    .Take(fCount)
                    .ToListAsync();
                query.AddRange(queryF);
            }
            return query;
        }

        /// <summary>
        /// 【验证器】更新代理的验证状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verifyState">验证状态 （1验证中，0待验证） 默认0</param>
        /// <returns></returns>
        public async Task<bool> UpdateProxyVerifyStateAsync(Guid id, int verifyState)
        {
            try
            {
                var proxy = await _db.Set<Proxies>().FirstOrDefaultAsync(f => f.Id == id);
                if (proxy == null) return false;

                proxy.VerifyState = verifyState;
                _db.Update(proxy);
                int ret = await _db.SaveChangesAsync();
                if (ret == 0) return false;
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 【验证器】结果队列专属更新代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateValidatorTaskResultAsync(ProxiesQueueModel model)
        {
            try
            {
                var proxy = await _db.Set<Proxies>().FirstOrDefaultAsync(f => f.Id == model.Id);
                if (proxy == null) return false;

                proxy.Validated = model.Success;
                proxy.Latency = model.Latency;
                proxy.ValidateDate = model.ValidateDate;
                proxy.ValidateFailedCnt = model.ValidateFailedCnt;
                proxy.ToValidateDate = model.ToValidateDate;
                proxy.VerifyState = 0; //重新设为待验证状态

                _db.Update(proxy);
                int ret = await _db.SaveChangesAsync();
                if (ret == 0) return false;
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 【验证器】批量删除代理
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(IEnumerable<Guid> ids)
        {
            try
            {
                var proxy = await _db.Set<Proxies>()
                    .Where(p => ids.Contains(p.Id)).ToListAsync();

                _db.RemoveRange(proxy);
                await _db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }

        #endregion 【验证器】

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

        /// <summary>
        /// 【爬取器】添加爬取的代理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> AddAsync(ProxiesFetcherModel model)
        {
            if (await _db.Set<Proxies>().AnyAsync(a => a.Ip == model.Ip && a.Port == model.Port))
            {
                return 0;
            }
            DateTime time = DateTime.Now.SetKindLocal();
            await _db.AddAsync(new Proxies()
            {
                Id = Guid.NewGuid(),
                FetcherName = model.FetcherName,
                Protocol = model.Protocol,
                Ip = model.Ip,
                Port = model.Port,
                Location = model.Location,
                Validated = false,
                ToValidateDate = time,
                ValidateFailedCnt = 0,
                VerifyState = 0
            });
            return await _db.SaveChangesAsync();
        }

    }
}
