using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UnitTest
{
    /// <summary>
    /// 测试主机
    /// </summary>
    public class TestHost
    {
        public static IHost Host { private set; get; }
        static TestHost()
        {
            Host = CreateHost();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return Host.Services.GetService<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetServices<T>()
        {
            return Host.Services.GetServices<T>();
        }
        /// <summary>
        /// 建立主机
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <param name="serviceDelegate"></param>
        /// <param name="loggingDelegate"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHost CreateHost()
        {
            var builder = CreateDefaultBuilder();
            var host = builder.Build();
            return host;
        }
        /// <summary>
        /// 建立主机生成器
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateDefaultBuilder()
        {
            IHostBuilder builder = new HostBuilder();

            builder.UseContentRoot(Directory.GetCurrentDirectory());

            builder.ConfigureAppConfiguration((context, config) => {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", true, true);
                config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                {
                    var assembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (assembly != null)
                    {
                        config.AddUserSecrets(assembly, true);
                    }
                }
                config.AddEnvironmentVariables();
            });

            builder.ConfigureLogging((context, logging) => {
                var configuration = context.Configuration;
                logging.AddConfiguration(configuration.GetSection("Logging"));
#if DEBUG
                logging.AddConsole();
                logging.AddDebug();
#endif
            });
            builder.UseDefaultServiceProvider((context, options) => {
                var isDevelopment = context.HostingEnvironment.IsDevelopment();
                options.ValidateScopes = isDevelopment;
                options.ValidateOnBuild = isDevelopment;
            });

            builder.ConfigureServices((context, services) => {
                // 添加资源限制相关配置
                services.AddLimitResource(context.Configuration);
                // 添加自动映射对像服务
                services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
                // 添加文件系统日志服务
                services.AddFileSystemLogging(context.Configuration);
                // 添加数据库上下文服务
                services.AddDBContext(context.Configuration);
                // 添加身份认证服务
                services.AddYDHAuthentication(context.Configuration);
                // 添加授权检查服务
                services.AddYDHAuthorization(context.Configuration);
                // 添加简单异常对象
                services.AddExceptionPOCO();
                // 添加YDH控制器服务
                services.AddYDHControllers();
                // 注册自动构建的服务
                services.RegisterServices();
                // 添加健康检查服务
                services.AddHealthChecks();
            });
            return builder;
        }
    }
}
