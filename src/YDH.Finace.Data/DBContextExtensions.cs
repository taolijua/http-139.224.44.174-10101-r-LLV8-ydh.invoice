using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YDH.Finace.Data;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Binder;
using YDH.Finace.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YDH.Port.Data.Repositories;
using System.Threading;

namespace YDH.Finace.Data
{
    /// <summary>
    /// 数据库访问上下文扩展方法
    /// </summary>
    public static class DBContextExtensions
    {
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 数据库访问上下文扩展方法
    /// </summary>
    public static class DBContextExtensions
    {
        /// <summary>
        /// 添加数据库访问上下文服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddDBContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DBContextOptions>(configuration.GetProtectedSection(nameof(DBContextOptions)));
            services.AddSingleton<IChangeTypeProvider, DefaultChangeTypeProvider>();
            services.AddSingleton<IEntityInfoManager, EntityInfoManager>();
            services.AddSingleton<IEntityBinder, DefaultEntityBinder>();
            services.AddScoped<ISqliteContext, SqliteContext>();
            services.AddScoped<IMySqlContext, MySqlContext>();
            services.AddScoped<ISqlServerContext, SqlServerContext>();
            return services;
        }
    }
}
