using Microsoft.Extensions.DependencyInjection;
using ProxyPool.Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProxyPoolServicesServiceCollectionExtensions
{
    /// <summary>
    /// 注册基础服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddProxyPoolBaseServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IProxyPoolService, ProxyPoolService>();
        // 当前用户
        //services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}

