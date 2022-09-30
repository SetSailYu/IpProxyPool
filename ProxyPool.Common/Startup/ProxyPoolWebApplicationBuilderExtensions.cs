using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using ProxyPool.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection;

public static class ProxyPoolWebApplicationBuilderExtensions
{
    /// <summary>
    /// 基本能力配置   
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder ProxyPoolBaseConfigure(this WebApplicationBuilder builder)
    {
        // 配置
        var configuration = builder.Configuration;
        AppSettingsConfig.Configure(configuration);

        // 支持提供 Windows-1252, Shift-JIS,  GB2312 三种编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return builder;
    }
}

