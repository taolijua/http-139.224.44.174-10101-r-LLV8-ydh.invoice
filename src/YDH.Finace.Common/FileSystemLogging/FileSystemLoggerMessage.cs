using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志消息对象
    /// </summary>
    internal sealed class FileSystemLoggerMessage
    {
        /// <summary>
        /// 日志文件，记录到那个文件
        /// </summary>
        public string File { set; get; }
        /// <summary>
        /// 日志内容，已完成格式化
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 异常ID，默认为0
        /// </summary>
        public int ExceptionId { set; get; }
        /// <summary>
        /// 是否创建新文件
        /// </summary>
        public bool IsNewFile { set; get; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(File) || string.IsNullOrEmpty(Content);
    }
}
