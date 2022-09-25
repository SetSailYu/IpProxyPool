using ProxyPoolAPI.PgDB;
using ProxyPoolAPI.Entity;

namespace ProxyPoolAPI.Repository
{
    /// <summary>
    /// 代理仓储
    /// </summary>
    public class ProxiesRepository : BaseRepository<Proxies>, IProxiesRepository
    {
        private readonly PgDBContext db;
        public ProxiesRepository(PgDBContext dbContext) : base(dbContext)
        {
            this.db = dbContext;
        }
    }
}
