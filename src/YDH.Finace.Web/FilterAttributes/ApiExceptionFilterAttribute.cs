using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YDH.Finace.Common.AllExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http.Extensions;

namespace YDH.Finace.Web.FilterAttributes
{
    /// <summary>
    /// 异常拦截器
    /// <para>此过滤器处理后的http status code为400</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        private ILogger _logger;
        /// <summary>
        /// 异常简单对象工厂 
        /// </summary>
        private IExceptionPOCOFactory _factory;

        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="context"></param>
        public override void OnException(ExceptionContext context)
        {
            Initialize(context);
            if (!context.ExceptionHandled)
            {
                var ex = context.Exception;
                var name = context.ActionDescriptor.DisplayName;
                var request = context.HttpContext.Request.GetDisplayUrl();
                var message = $"Name:{name}\r\nRequest:{request}\r\nMessage:{ex?.Message}";
                var resultObject = _factory.Create(ex);
                if (!resultObject.IgnoreLogging) _logger.LogError(ex, message);
                context.Result = new ObjectResult(resultObject) { StatusCode = 400 };
                
            }
            else
            {
                base.OnException(context);
            }
        }

        /// <summary>
        /// 请求过程中初始化全局对象
        /// </summary>
        /// <param name="context"></param>
        private void Initialize(ExceptionContext context)
        {
            // 日志记录器
            if (_logger == null)
            {
                var logFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                if (context.ActionDescriptor is ControllerActionDescriptor controllerDescriptor)
                {
                    _logger = logFactory.CreateLogger(controllerDescriptor.ControllerTypeInfo.AsType());
                }
                else
                {
                    _logger = logFactory.CreateLogger<ApiExceptionFilterAttribute>();
                }
            }
            // 
            _factory ??= context.HttpContext.RequestServices.GetRequiredService<IExceptionPOCOFactory>();

        }
    }


}
