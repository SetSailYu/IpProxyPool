using Microsoft.EntityFrameworkCore;
using ProxyPool.Common;
using ProxyPool.Common.Helpers;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Enum;
using ProxyPool.Services.BackgroundTasks;
using ProxyPoolAPI.Tasks;

var builder = WebApplication.CreateBuilder(args);

// 基本能力配置
builder.ProxyPoolBaseConfigure();

var configuration = builder.Configuration;

// postgres数据库依赖注入 
//builder.Services.AddDbContextPool<PgDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PgDB")));
// 仓储层依赖注入
//builder.Services.AddScoped<IEntityRepository, BaseEntityRepository>();


// Add services to the container.
builder.Services.AddControllers();
// API
//builder.Services.AddControllers()
//    //.AddDataValidation()
//    .AddAppResult(options =>
//    {
//        options.ResultFactory = resultException =>
//        {
//            // AppResultException 都返回 200 状态码
//            var objectResult = new ObjectResult(resultException.AppResult);
//            objectResult.StatusCode = StatusCodes.Status200OK;
//            return objectResult;
//        };
//    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 仓储层
builder.Services.AddRepository(SqlType.PostgreSql, configuration.GetConnectionString("PostgreSql"));

// 服务层：添加基础服务
builder.Services.AddProxyPoolBaseServices();
// 服务层：自动添加 Service 层以 Service 结尾的服务
builder.Services.AddAutoServices("ProxyPool.Services");

// 对象映射 AutoMapper
var profileAssemblies = AssemblyHelper.GetAssemblies("ProxyPool.Services");
builder.Services.AddAutoMapper(profileAssemblies, ServiceLifetime.Singleton);

// 跨域
builder.Services.AddProxyPoolCors();

// 爬取器任务
builder.Services.AddHostedService<FetcherTask>();

//ConsoleHelper.WriteSuccessLog("Task ========>");

//List<ITaskService> services = new List<ITaskService>()
//{
//    new FetcherTaskService(builder.Services.BuildServiceProvider().GetService<DB>())
//};

//foreach (var serv in services)
//{
//    System.Threading.Tasks.Task.Run(serv.Execute);
//}

//ConsoleHelper.WriteSuccessLog("Build ========>");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// UseCors 必须在 UseRouting 之后，UseResponseCaching、UseAuthorization 之前
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
