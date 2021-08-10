using Microsoft.Extensions.DependencyInjection.Extensions;
using YDH.Finace.Common.AllExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExceptionPOCOExtensions
    {
        /// <summary>
        /// 添加异常简单对象工厂到容器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddExceptionPOCO(this IServiceCollection services)
        {
            services.TryAddSingleton<IExceptionPOCOFactory, ExceptionPOCOFactory>();
            return services;
        }
    }
}
