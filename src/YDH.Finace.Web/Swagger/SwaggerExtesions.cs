using Microsoft.OpenApi.Models;
using YDH.Finace.Common;
using YDH.Finace.Common.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YDH.Finace.Web.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace YDH.Finace.Common.Swagger
{
    /// <summary>
    /// 
    /// </summary>
    public static class SwaggerExtesions
    {
    }
}
namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public static class SwaggerExtesions
    {
        private static byte[] bytes;
        private static readonly string index = @"AppFiles\Swagger\default.html";
        private static MemoryStream Index => new MemoryStream(bytes ??= File.ReadAllBytes(index));

        /// <summary>
        /// 使用api文档
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseApiDocument(this IApplicationBuilder builder)
        {
            return builder.UseSwagger().UseSwaggerUI(g => {
                g.SwaggerEndpoint("/swagger/v1/swagger.json", "YDH服务");
                g.DocumentTitle = "API Document";
                g.DefaultModelsExpandDepth(-1);
                g.IndexStream = () => Index;
            });
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public static class SwaggerExtesions
    {
        /// <summary>
        /// 使用swagger服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            //var assembly = Assembly.GetCallingAssembly();
            //var assemblyRoot = assembly.GetDirectory();
            //var commentsXmlFile = $"{assembly.GetName().Name}.xml";
            //var commentsXmlPath = Path.Combine(assemblyRoot, commentsXmlFile);
            var authOptions = configuration.GetYDHAuthOptions();

            return services.AddSwaggerGen(g =>
            {
                g.OperationFilter<AuthenticationHeaderFilter>(authOptions.TokenName);
                Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory, "*.xml").ToList().ForEach(x => g.IncludeXmlComments(System.IO.Path.Combine(x)));//处理xml文件
                //g.IncludeXmlComments(commentsXmlPath);
                g.DocumentFilter<HiddenApiFilter>();
                g.SchemaFilter<HiddenApiFilter>();
                g.SwaggerDoc("v1", new OpenApiInfo { Title = "YDH服务", Version = "v1" });
            });
        }
    }
}