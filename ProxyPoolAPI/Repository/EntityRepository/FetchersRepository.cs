using ProxyPoolAPI.DB;
using ProxyPoolAPI.Entity;

namespace ProxyPoolAPI.Repository
{
    /// <summary>
    /// 爬取器仓储
    /// </summary>
    public class FetchersRepository : BaseRepository<Fetchers>, IFetchersRepository
    {
        private readonly PgDBContext db;
        public FetchersRepository(PgDBContext dbContext) : base(dbContext)
        {
            this.db = dbContext;
        }

    }
}
