using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YDH.Finace.Common;
using YDH.Finace.Common.Cache;

namespace YDH.Finace.App
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 构建实例
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostEnvironment"></param>
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
            if(configuration.GetValue<bool>("TraceInfo"))
                Trace.Listeners.Add(new ConsoleTraceListener());
        }
        /// <summary>
        /// 当前配置集合
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 当前通用主机环境
        /// </summary>
        public IHostEnvironment HostEnvironment { get; }


        /// <summary>
        /// 配置服务容器
        /// <see cref="IHostBuilder.Build"/>运行此方法
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // 控制时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // 添加http 上下文
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // 添加后台工作服务
            //  services.AddBackgroundWorkService();
            services.AddMemoryCache();
            // 内存缓存认证注入
            services.AddTransient<ICacheService,MemoryCacheService>();  
            // 添加资源限制相关配置
            services.AddLimitResource(Configuration);
            // 添加自动映射对像服务
            services.AddAutoMapper(AppDomain.CurrentDomain.GetYDHAssemblies());
            // 添加文件系统日志服务
            services.AddFileSystemLogging(Configuration);
            // 添加数据保护服务
            services.AddDataProtection(HostEnvironment);
            // 添加后台工作项服务
           // services.AddWorkService(Configuration);
            // 添加数据库上下文服务
            services.AddDBContext(Configuration);
            // 添加身份认证服务
            services.AddYDHAuthentication(Configuration);
            // 添加授权检查服务
            services.AddYDHAuthorization(Configuration);
            // 添加简单异常对象
            services.AddExceptionPOCO();
            // 添加YDH控制器服务
            services.AddYDHControllers();
            // 注册自动构建的服务
            services.RegisterServices();
            // 添加健康检查服务
            services.AddHealthChecks();
            // 添加自动文档生成服务
            services.AddSwagger(Configuration);
            
        }

        /// <summary>
        /// 配置请求管道
        /// <para><see cref="IWebHost"/>.Run() 运行此方法</para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="lifetime"></param>
        /// <param name="logger"></param>
        public void Configure(IApplicationBuilder app, IHostEnvironment env, IHostApplicationLifetime lifetime, ILogger<Startup> logger)
        {
            lifetime.ApplicationStarted.Register(() => logger.LogInformation("Started"));
            lifetime.ApplicationStopping.Register(() => logger.LogInformation("Stopping"));
            lifetime.ApplicationStopped.Register(() => logger.LogInformation("Stopped"));
            logger.LogInformation($"{nameof(Startup)}\r\n" +
                                  $"ApplicationName: {env.ApplicationName}\r\n" +
                                  $"ContentRootPath: {env.ContentRootPath}\r\n" +
                                  $"EnvironmentName: {env.EnvironmentName}");

            // 请求并发限制
            app.UseLimitRequest();
            // 静态文件中间件
            app.UseCustomStaticFiles();
            // 拒绝网站图标中间件
            app.UseDeniedFavoriteIcon();
            // 未处理异常中间件
            app.UseUnhandledException();
            // 使用接口文档访问
            app.UseApiDocument();

            // 路由中间件
            app.UseRouting();
            // 跨域中间件
            app.UseCors();
            // 限制中间件
            app.UseLimitResources();


            // 认证中间件
            app.UseAuthentication();
            // 授权中间件
            app.UseAuthorization();

            // 终结点
            app.UseEndpoints(endpoints =>
            {
                // 属性路由
                endpoints.MapControllers();
                // 健康检查路由
                endpoints.MapHealthChecks("/Health");

               

            });
            logger.LogInformation("Configured");
        }
    }
}
