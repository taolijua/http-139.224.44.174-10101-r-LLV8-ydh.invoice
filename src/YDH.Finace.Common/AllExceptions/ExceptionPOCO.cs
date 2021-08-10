using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.AllExceptions
{
    /// <summary>
    /// 异常简单对象
    /// </summary>
    public sealed class ExceptionPOCO
    {
        /// <summary>
        /// 异常消息
        /// </summary>
        public string Message { set; get; }
        /// <summary>
        /// 堆栈跟踪
        /// </summary>
        //public string StackTrace { set; get; }
        /// <summary>
        /// 忽略日志
        /// </summary>
        public bool IgnoreLogging { set; get; }
    }
}
