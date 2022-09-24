using Microsoft.AspNetCore.Mvc;
using ProxyPoolAPI.DB;
using ProxyPoolAPI.Repository;

namespace ProxyPoolAPI.Controllers
{
    /// <summary>
    /// 基础控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        /// <summary>
        /// 实体仓储
        /// </summary>
        public readonly BaseEntityRepository er;
        public readonly PgDBContext db;
        public BaseController(PgDBContext dbContext)
        {
            er = new BaseEntityRepository(dbContext);
            db = dbContext;
        }
    }
}
