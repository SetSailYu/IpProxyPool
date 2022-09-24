using Microsoft.EntityFrameworkCore;
using ProxyPoolAPI.DB;
using ProxyPoolAPI.Entity;

namespace ProxyPoolAPI.Repository
{
    /// <summary>
    /// 实体仓储接口
    /// </summary>
    public interface IEntityRepository
    {
        /// <summary>
        /// 爬取器仓储接口
        /// </summary>
        IFetchersRepository Fetchers { get; }
        /// <summary>
        /// 代理仓储接口
        /// </summary>
        IProxiesRepository Proxies { get; }
    }
    /// <summary>
    /// 实体仓储
    /// </summary>
    public class BaseEntityRepository : IEntityRepository
    {
        private readonly PgDBContext _dbContext;
        public BaseEntityRepository(PgDBContext dbContext) => _dbContext = dbContext;

        public IFetchersRepository Fetchers => new FetchersRepository(_dbContext);

        public IProxiesRepository Proxies => new ProxiesRepository(_dbContext);
    }

    public interface IFetchersRepository : IRepository<Fetchers>
    {
    }
    
    public interface IProxiesRepository : IRepository<Proxies>
    {
    }
    



}
