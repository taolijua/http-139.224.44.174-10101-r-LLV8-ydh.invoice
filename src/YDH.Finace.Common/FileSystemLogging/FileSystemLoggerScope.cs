using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 模仿 Microsoft.Extensions.Logging.Abstractions.Internal.NullScope(2.2)
    /// </summary>
    internal class FileSystemLoggerScope : IDisposable
    {
        /// <summary>
        /// 静态的
        /// </summary>
        public static FileSystemLoggerScope Scope { get; } = new FileSystemLoggerScope();
        /// <summary>
        /// 私有构造
        /// </summary>
        private FileSystemLoggerScope() { }
        /// <summary>
        /// 此方法为空
        /// </summary>
        public void Dispose() { }
    }
}
