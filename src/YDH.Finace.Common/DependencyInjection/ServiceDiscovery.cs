using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common;
using YDH.Finace.Common.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 服务发现扩展
    /// </summary>
    public static class ServiceDiscoveryExtensions
    {
        private static readonly Type scopedType = typeof(IScoped);
        private static readonly Type serviceType = typeof(IService);
        private static readonly Type transientType = typeof(ITransient);
        private static readonly Type multipleImplType = typeof(IMultipleImplService);

        /// <summary>
        /// 注册当前应用程序中预置好的服务
        /// </summary>
        /// <param name="services"></param>
        public static void RegisterServices(this IServiceCollection services)
        {
            var serviceCount = AppDomain.CurrentDomain.GetAssemblies().
                               Where(assembly => assembly.FullName.StartsWith("YDH.")).
                               SelectMany(assembly => assembly.GetExportedTypes()).
                               Count(type => services.RegisterService(type));
            //Trace.WriteLine($"ServiceCount:{serviceCount}");
        }
        /// <summary>
        /// 检查服务并注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool RegisterService(this IServiceCollection services, Type type)
        {
            if (type.IsClass &&
                type.IsPublic &&
                !type.IsAbstract &&
                !type.IsInterface &&
                serviceType.IsAssignableFrom(type))
            {
                var interfaces = type.GetInterfaces();
                var definition = interfaces.FirstOrDefault(@interface => @interface.Name.EndsWith(type.Name));
                if (definition == null)
                {
                    var definitions = interfaces.Where(@interface => !serviceType.Equals(@interface) &&
                                                                     serviceType.IsAssignableFrom(@interface)).ToList();
                    if (definitions.Count == 1)
                    {
                        definition = definitions.First();
                    }
                    else if (definitions.Count > 1)
                    {
                        definition = definitions.InheritSort().First();
                    }
                }
                if (definition != null)
                {
                    //Trace.WriteLine($"Definition:{definition.Name} => Impl:{type.Name}");
                    // 是否调置为一个服务类型可注册多个服务实现
                    if (interfaces.Any(@interface => multipleImplType.Equals(@interface)))
                    {
                        if (interfaces.Any(@interface => transientType.Equals(@interface)))
                        {
                            services.AddTransient(definition, type);
                        }
                        else if (interfaces.Any(@interface => scopedType.Equals(@interface)))
                        {
                            services.AddScoped(definition, type);
                        }
                        else
                        {
                            services.AddSingleton(definition, type);
                        }
                    }
                    else
                    {
                        if (interfaces.Any(@interface => transientType.Equals(@interface)))
                        {
                            services.TryAddTransient(definition, type);
                        }
                        else if (interfaces.Any(@interface => scopedType.Equals(@interface)))
                        {
                            services.TryAddScoped(definition, type);
                        }
                        else
                        {
                            services.TryAddSingleton(definition, type);
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
