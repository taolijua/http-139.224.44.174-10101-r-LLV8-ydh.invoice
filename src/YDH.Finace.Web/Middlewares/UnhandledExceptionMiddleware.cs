using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.Web.Middlewares
{
    /// <summary>
    /// 未处理异常拦截中间件
    /// </summary>
    public sealed class UnhandledExceptionMiddleware
    {
        /// <summary>
        /// 是否显示错误详情
        /// </summary>
        private readonly bool showExceptionDetail;
        /// <summary>
        /// 下一个中间件
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="env"></param>
        public UnhandledExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            showExceptionDetail = env.IsDevelopment();
            _next = next;
        }
        /// <summary>
        /// 通过try catch块捕获未处理异常
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var path = context.Request.Path;
                var message = showExceptionDetail ? ex.ToString() : ex.Message;
                context.RequestServices.GetService<ILogger<UnhandledExceptionMiddleware>>()?.DailyRollFile(path, ex);
                if (path.IsWebApi())
                {
                    await context.Response.WriteAsync($"{{\"message\":\"{message.Replace(Environment.NewLine, "\\r\\n")}\"}}");
                }
                else
                {
                    await context.Response.WriteAsync($"<!DOCTYPE html><html><body><div><pre>{message}</pre></div></body></html>");
                }
            }
        }

    }
}
