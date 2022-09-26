using Microsoft.EntityFrameworkCore;
using ProxyPool.Common;
using ProxyPool.Common.Helpers;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Enum;
using ProxyPool.Services.BackgroundTasks;
using ProxyPoolAPI.Tasks;

var builder = WebApplication.CreateBuilder(args);

// ������������
builder.ProxyPoolBaseConfigure();

var configuration = builder.Configuration;

// postgres���ݿ�����ע�� 
//builder.Services.AddDbContextPool<PgDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PgDB")));
// �ִ�������ע��
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
//            // AppResultException ������ 200 ״̬��
//            var objectResult = new ObjectResult(resultException.AppResult);
//            objectResult.StatusCode = StatusCodes.Status200OK;
//            return objectResult;
//        };
//    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// �ִ���
builder.Services.AddRepository(SqlType.PostgreSql, configuration.GetConnectionString("PostgreSql"));

// ����㣺��ӻ�������
builder.Services.AddProxyPoolBaseServices();
// ����㣺�Զ���� Service ���� Service ��β�ķ���
builder.Services.AddAutoServices("ProxyPool.Services");

// ����ӳ�� AutoMapper
var profileAssemblies = AssemblyHelper.GetAssemblies("ProxyPool.Services");
builder.Services.AddAutoMapper(profileAssemblies, ServiceLifetime.Singleton);

// ����
builder.Services.AddProxyPoolCors();

// ��ȡ������
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

// UseCors ������ UseRouting ֮��UseResponseCaching��UseAuthorization ֮ǰ
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
