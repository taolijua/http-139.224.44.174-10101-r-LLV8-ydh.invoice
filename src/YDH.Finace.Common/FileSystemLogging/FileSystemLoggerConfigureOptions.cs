using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志配置选项
    /// </summary>
    public sealed class FileSystemLoggerConfigureOptions
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel MinLogLevel { set; get; } = LogLevel.Information;
        /// <summary>
        /// 日志根路径
        /// </summary>
        public string RootPath { set; get; } = "Log";
        /// <summary>
        /// 日志文件编码
        /// </summary>
        public string Encoding { set; get; } = "UTF-8";
        /// <summary>
        /// 日志文件扩展名
        /// </summary>
        public string Extension { set; get; } = "log";
        /// <summary>
        /// 日志队列消费延迟
        /// </summary>
        public int QueueConsumeDelay { set; get; } = 250;
        /// <summary>
        /// 最大文件大小
        /// </summary>
        public int MaxFileSize { set; get; } = 10000000;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FileSystemLoggerConfigureOptions Extend()
        {
            var config =  new FileSystemLoggerConfigureOptions
            {
                MinLogLevel = MinLogLevel == LogLevel.None ? LogLevel.Information : MinLogLevel,
                QueueConsumeDelay = QueueConsumeDelay < 250 ? 250 : QueueConsumeDelay,
                MaxFileSize = MaxFileSize < 1000 * 1000 ? 1000 * 1000 : MaxFileSize,
                Extension = Extension.IsEmpty() ? "log" : Extension,
                Encoding = Encoding.IsEmpty() ? "UTF-8" : Encoding,
                RootPath = RootPath.IsEmpty() ? "Log" : RootPath
            };
            if (!Path.IsPathFullyQualified(config.RootPath))
                config.RootPath = Path.Combine(AppContext.BaseDirectory, config.RootPath);

            return config;
        }

    }
}
