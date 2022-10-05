using Microsoft.AspNetCore.Mvc;
using ProxyPool.Common;
using ProxyPool.Repository.Entity;
using ProxyPool.Services;

namespace ProxyPoolAPI.Controllers
{
    /// <summary>
    /// 代理控制器 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProxiesController : ControllerBase
    {
        private readonly ProxiesService _service;
        public ProxiesController(ProxiesService service)
        {
            _service = service;
        }


        /// <summary>
        /// 获取可用代理数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppResult> GetAllTrueSum()
        {
            int data = await _service.GetAllTrueSum();
            return AppResult.Status200OK(data: data);
        }

    }
}
