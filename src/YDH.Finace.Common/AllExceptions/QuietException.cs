using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.AllExceptions
{
    /// <summary>
    /// 当前程序需要静默（只记录日志或打印调试日志）处理的异常
    /// </summary>
    public sealed class QuietException : BaseException
    {
        /// <summary>
        /// 初始化 <see cref="QuietException"/> 类的新实例
        /// </summary>
        public QuietException() : base($"产生了应用程序异常") { }
        /// <summary>
        /// 使用指定的错误信息初始化 <see cref="QuietException"/> 类的新实例
        /// </summary>
        /// <param name="message"></param>
        public QuietException(string message) : base(message) { }
        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="QuietException"/> 类的新实例。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public QuietException(string message, Exception innerException) : base(message, innerException) { }
    }
}
