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
        /// ����ʵ��
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
        /// ��ǰ���ü���
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// ��ǰͨ����������
        /// </summary>
        public IHostEnvironment HostEnvironment { get; }


        /// <summary>
        /// ���÷�������
        /// <see cref="IHostBuilder.Build"/>���д˷���
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // ����ʱ���ʽ
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            });
            // ���http ������
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // ��Ӻ�̨��������
            //  services.AddBackgroundWorkService();
            services.AddMemoryCache();
            // �ڴ滺����֤ע��
            services.AddTransient<ICacheService,MemoryCacheService>();  
            // �����Դ�����������
            services.AddLimitResource(Configuration);
            // ����Զ�ӳ��������
            services.AddAutoMapper(AppDomain.CurrentDomain.GetYDHAssemblies());
            // ����ļ�ϵͳ��־����
            services.AddFileSystemLogging(Configuration);
            // ������ݱ�������
            services.AddDataProtection(HostEnvironment);
            // ��Ӻ�̨���������
           // services.AddWorkService(Configuration);
            // ������ݿ������ķ���
            services.AddDBContext(Configuration);
            // ��������֤����
            services.AddYDHAuthentication(Configuration);
            // �����Ȩ������
            services.AddYDHAuthorization(Configuration);
            // ��Ӽ��쳣����
            services.AddExceptionPOCO();
            // ���YDH����������
            services.AddYDHControllers();
            // ע���Զ������ķ���
            services.RegisterServices();
            // ��ӽ���������
            services.AddHealthChecks();
            // ����Զ��ĵ����ɷ���
            services.AddSwagger(Configuration);
            
        }

        /// <summary>
        /// ��������ܵ�
        /// <para><see cref="IWebHost"/>.Run() ���д˷���</para>
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

            // ���󲢷�����
            app.UseLimitRequest();
            // ��̬�ļ��м��
            app.UseCustomStaticFiles();
            // �ܾ���վͼ���м��
            app.UseDeniedFavoriteIcon();
            // δ�����쳣�м��
            app.UseUnhandledException();
            // ʹ�ýӿ��ĵ�����
            app.UseApiDocument();

            // ·���м��
            app.UseRouting();
            // �����м��
            app.UseCors();
            // �����м��
            app.UseLimitResources();


            // ��֤�м��
            app.UseAuthentication();
            // ��Ȩ�м��
            app.UseAuthorization();

            // �ս��
            app.UseEndpoints(endpoints =>
            {
                // ����·��
                endpoints.MapControllers();
                // �������·��
                endpoints.MapHealthChecks("/Health");

               

            });
            logger.LogInformation("Configured");
        }
    }
}
