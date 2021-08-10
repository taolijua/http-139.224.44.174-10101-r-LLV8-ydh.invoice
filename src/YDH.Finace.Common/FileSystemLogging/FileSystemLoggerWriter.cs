using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志写入对象
    /// </summary>
    internal sealed class FileSystemLoggerWriter
    {

        /// <summary>
        /// 文件写入编码
        /// </summary>
        private readonly Encoding encoding;
        /// <summary>
        /// 文件系统日志配置选项
        /// </summary>
        private readonly FileSystemLoggerConfigureOptions _config;
        /// <summary>
        /// 此写入器应由<see cref="FileSystemLoggerQueue"/>创建
        /// </summary>
        /// <param name="config"></param>
        public FileSystemLoggerWriter(FileSystemLoggerConfigureOptions config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            if (!Encoding.GetEncodings().Any(i => i.Name.Equals(config.Encoding, StringComparison.OrdinalIgnoreCase)))
            {
                // 注册额外代码页
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            encoding = Encoding.GetEncoding(config.Encoding);
        }

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task WriteMessageAsync(FileSystemLoggerMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var fullPath = message.File;
            if (!Path.IsPathFullyQualified(message.File)) fullPath = Path.Combine(_config.RootPath, message.File);
            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            await WriteMessageAsync(fullPath, message);
        }
        /// <summary>
        /// 写入消息，此处检查文件大小，如超过阀值则移动文件
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task WriteMessageAsync(string fullPath, FileSystemLoggerMessage message)
        {
            var info = new FileInfo(fullPath);
            if (info.Exists && info.Length >= _config.MaxFileSize)
            {
                var newName = $"{Path.GetFileNameWithoutExtension(info.FullName)}_{DateTime.Now.Ticks}.{_config.Extension}";
                var newFile = Path.Combine(Path.GetDirectoryName(info.FullName), newName);
                info.MoveTo(newFile);
            }
            await WriteMessageAsync(info, message);
        }
        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task WriteMessageAsync(FileInfo info, FileSystemLoggerMessage message)
        {
            var mode = message.IsNewFile ? FileMode.Create : FileMode.Append;
            using var fileStream = info.Open(mode, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fileStream, encoding);
            await writer.WriteAsync(message.Content);
        }
    }
}
