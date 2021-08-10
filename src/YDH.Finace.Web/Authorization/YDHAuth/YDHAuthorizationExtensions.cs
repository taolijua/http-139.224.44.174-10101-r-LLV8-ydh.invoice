using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using YDH.Finace.Web.Authentication.YDHAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using YDH.Finace.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class YDHAuthorizationExtensions
    {
          
        /// <summary>
        /// 添加YDH授权验证
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddYDHAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var workOperatorName = configuration.GetYDHAuthOptions().WorkOperatorAccount;
            services.AddAuthorization(options =>
            {
                // 基本用户策略  存在用户名
                options.AddPolicy(YDHAuthOptions.BasicUser, p => p.RequireAssertion(context => context.User.HasIdentityName()));
                // 系统用户策略  存在用户ID
                options.AddPolicy(YDHAuthOptions.SystemUser, p => p.RequireClaim(YDHAuthOptions.UserIdClaimType));
                // 工作操作策略  存在指定用户名
                //if (workOperatorName.HasValue())
                //   options.AddPolicy(YDHAuthOptions.WorkOperator, p => p.RequireUserName(workOperatorName));

                // 此策略要求授权对象必须是认证用户
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build() ;
                // 默认策略本来就是，这里显式替换，终结点不存在IAuthorizeData实现，则使用回退策略验证授权
                options.DefaultPolicy = policy;
                // 回退策略本来为空，替换后所有终结点元数据不存在IAllowAnonymous实现，则都需要认证用户
                options.FallbackPolicy = policy;
            });
            return services;
        }
    }
}
