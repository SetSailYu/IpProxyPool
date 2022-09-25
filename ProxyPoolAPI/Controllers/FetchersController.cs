using Microsoft.AspNetCore.Mvc;
using ProxyPool.Common;
using ProxyPool.Repository.Entity;
using ProxyPool.Services;
using ProxyPoolAPI.PgDB;
using ProxyPoolAPI.Repository;
using System.ComponentModel.Design;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProxyPoolAPI.Controllers
{
    /// <summary>
    /// 抓取器
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FetchersController : ControllerBase
    {
        private readonly FetchersService _service;
        public FetchersController(FetchersService service)
        {
            _service = service; 
        }

        ///// <summary>
        ///// 抓取器测试
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public JsonResult Get()
        //{
        //    //Fetchers fetchers = er.Fetchers.GetAll().FirstOrDefault() ?? new Fetchers();
        //    Fetchers fetchers = db.Fetchers.FirstOrDefault() ?? new Fetchers();
        //    return Json(fetchers);
        //}

        /// <summary>
        /// 抓取器测试
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppResult> List()
        {
            List<Fetchers> data = await _service.GetAsync();
            return AppResult.Status200OK(data: data);
        }

        /// <summary>
        /// 抓取器分页查询测试
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AppResult> Page([FromQuery] PageRequestModel model)
        {
            PageResultModel<Fetchers> data = await _service.GetPageAsync(model);
            return AppResult.Status200OK(data: data);
        }

        //// GET api/<FetchersController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<FetchersController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<FetchersController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<FetchersController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
