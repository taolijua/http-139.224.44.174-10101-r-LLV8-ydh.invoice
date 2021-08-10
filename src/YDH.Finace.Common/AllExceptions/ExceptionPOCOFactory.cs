using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.AllExceptions
{
    /// <summary>
    /// 异常简单对象工厂
    /// </summary>
    public interface IExceptionPOCOFactory
    {
        /// <summary>
        /// 创建异常简单对象
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        ExceptionPOCO Create(Exception ex, string message = null);
    }

    /// <summary>
    /// 异常简单对象工厂默认实现
    /// </summary>
    public class ExceptionPOCOFactory : IExceptionPOCOFactory
    {
        private readonly ILogger _logger;
        /// <summary>
        /// 是否开发模式
        /// </summary>
        private readonly bool _isDevelopment;
        /// <summary>
        /// 构建实例，应该为单例注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="env"></param>
        public ExceptionPOCOFactory(ILogger<ExceptionPOCOFactory> logger, IHostEnvironment env)
        {
            _logger = logger;
            _isDevelopment = env.IsDevelopment();
        }

        /// <summary>
        /// 忽略日志
        /// </summary>
        public bool IgnoreLogging { private set; get; }

        /// <summary>
        /// 创建异常简单对象
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ExceptionPOCO Create(Exception ex, string message = null)
        {
            var _message = message ?? string.Empty;
          //  var _stackTrace = string.Empty;
            var _ignorelogging = false;
            if (ex != null)
            {
               // _stackTrace = _isDevelopment ? ex.StackTrace : null;
                switch (ex)
                {
                    case FriendlyException _:
                    case ArgumentException _:
                        _message += ex.Message;
#if !DEBUG
                        _ignorelogging = true;
#endif
                        break;
                    case OperationCanceledException _:
   
                        break;
                    case QuietException _:
                    default:
                        break;
                }
            }
            return new ExceptionPOCO
            {
                Message = _message,
               // StackTrace = _stackTrace,
                IgnoreLogging = _ignorelogging
            };
        }
    }
}
