using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Repository.Base;
using ProxyPool.Repository.Enum;

namespace Microsoft.Extensions.DependencyInjection;

public static class EfCoreServiceCollectionExtensions
{
    /// <summary>
    /// 添加仓储设置
    /// </summary>
    /// <param name="services"></param>
    /// <param name="sqlName"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepository(this IServiceCollection services, SqlType sqlType, string connectionString)
    {
        services.AddDbContext<DB>(options =>
        {
            switch (sqlType)
            {
                case SqlType.PostgreSql:
                    options.UseNpgsql(connectionString);
                    break;
                case SqlType.SqlServer:
                    //options.UseSqlServer(connectionString);
                    break;
                case SqlType.MySql:
                    break;
                case SqlType.Oracle:
                    break;
                default:
                    return;
            }

            options.ConfigureWarnings(builder =>
            {
                // 消除 https://go.microsoft.com/fwlink/?linkid=2131316
                builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        });
        //var db = services.BuildServiceProvider().GetService<DB>();

        return services;
    }
}

