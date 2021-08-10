using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.AllExceptions
{
    /// <summary>
    /// 具有友好提示消息的异常
    /// </summary>
    public sealed class FriendlyException : BaseException
    {
        /// <summary>
        /// 初始化 <see cref="FriendlyException"/> 类的新实例
        /// </summary>
        public FriendlyException() : base($"产生了应用程序异常") { }
        /// <summary>
        /// 使用指定的错误信息初始化 <see cref="FriendlyException"/> 类的新实例
        /// </summary>
        /// <param name="message"></param>
        public FriendlyException(string message) : base(message) { }
        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="FriendlyException"/> 类的新实例。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public FriendlyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
