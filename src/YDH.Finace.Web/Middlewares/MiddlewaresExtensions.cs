using Microsoft.Extensions.FileProviders;
using YDH.Finace.Web;
using YDH.Finace.Web.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YDH.Finace.Common;
using Microsoft.AspNetCore.Rewrite;

namespace Microsoft.AspNetCore.Builder
{
    public static class MiddlewaresExtensions
    {
        /// <summary>
        /// 使用未处理异常拦截中间件
        /// </summary>
        /// <param name="builder"></param>
        public static void UseUnhandledException(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<UnhandledExceptionMiddleware>();
        }
        /// <summary>
        /// 拒绝收藏夹图标
        /// </summary>
        /// <param name="builder"></param>
        public static void UseDeniedFavoriteIcon(this IApplicationBuilder builder)
        {
            // builder.Build() 默认为http 404
            builder.MapWhen(http => http.Request.Path.StartsWith("/favicon.ico"), builder => builder.Build());
        }

        /// <summary>
        /// 使用自定义静态文件访问
        /// </summary>
        /// <param name="builder"></param>
        public static void UseCustomStaticFiles(this IApplicationBuilder builder)
        {
            builder.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } });
            var rewriteOptions = new RewriteOptions();
            //rewriteOptions.AddRewrite("index", "indexCase.html", true);
            rewriteOptions.AddRewrite("^/views/(.*)\\.html", "/views/$1.html", true);
            rewriteOptions.AddRewrite("^/page/(.*)\\.html", "/page/$1.html", true);
            builder.UseRewriter(rewriteOptions);
            builder.UseStaticFiles();
        }
    }
}
