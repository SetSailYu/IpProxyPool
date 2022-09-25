using Microsoft.EntityFrameworkCore;
using ProxyPoolAPI.Entity;

namespace ProxyPoolAPI.PgDB
{
    /// <summary>
    /// postgresSQL 操作上下文
    /// </summary>
    public class PgDBContext: DbContext
    {
        public PgDBContext(DbContextOptions<PgDBContext> options): base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fetchers>()
                .HasKey(c => new { c.Name });

            modelBuilder.Entity<Proxies>()
               .HasKey(c => new { c.Protocol, c.Ip, c.Port });

        }

        /// <summary>
        /// 代理
        /// </summary>
        public virtual DbSet<Proxies> Proxies { get; set; }
        /// <summary>
        /// 爬取器
        /// </summary>
        public virtual DbSet<Fetchers> Fetchers { get; set; }

    }
}
