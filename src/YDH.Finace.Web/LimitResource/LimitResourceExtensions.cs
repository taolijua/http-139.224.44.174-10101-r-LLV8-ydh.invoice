using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YDH.Finace.Web.LimitResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 限制资源扩展
    /// </summary>
    public static class LimitResourceExtensions
    {
        /// <summary>
        /// 添加限制资源服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddLimitResource(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LimitResourceOptions>(configuration.GetSection(nameof(LimitResourceOptions)));

            return services;
        }
    }
}

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 限制资源扩展
    /// </summary>
    public static class LimitResourceExtensions
    {
        /// <summary>
        /// 使用限制资源中间件
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLimitResources(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<LimitResourceMiddleware>();
            return builder;
        }

        /// <summary>
        /// 使用限制请求中间件(并发)
        /// <par>可以工作在其它中间件之前</par>
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLimitRequest(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LimitRequestMiddleware>();
        }
    }
}
