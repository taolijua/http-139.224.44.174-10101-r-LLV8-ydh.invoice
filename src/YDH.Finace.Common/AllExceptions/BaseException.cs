using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace YDH.Finace.Common.AllExceptions
{
    /// <summary>
    /// 各种由当前应用程序直接或间接引发的异常基类
    /// </summary>
    public abstract class BaseException : ApplicationException
    {
        /// <summary>
        /// 初始化 <see cref="BaseException"/> 类的新实例
        /// </summary>
        public BaseException() : base() { }
        /// <summary>
        /// 使用指定的错误信息初始化 <see cref="BaseException"/> 类的新实例
        /// </summary>
        /// <param name="message"></param>
        public BaseException(string message) : base(message) { }
        /// <summary>
        /// 使用指定错误消息和对作为此异常原因的内部异常的引用来初始化 <see cref="BaseException"/> 类的新实例。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public BaseException(string message, Exception innerException) : base(message, innerException) { }
    }
}
