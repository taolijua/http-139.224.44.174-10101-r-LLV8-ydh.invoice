using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志记录器
    /// </summary>
    internal sealed class FileSystemLogger : ILogger
    {
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { private set; get; }
        /// <summary>
        /// 当前记录器关联的队列，由<see cref="FileSystemLoggerProvider"/>提供
        /// </summary>
        private readonly FileSystemLoggerQueue _queue;
        /// <summary>
        /// 当前记录器关联的配置，由<see cref="FileSystemLoggerProvider"/>提供
        /// </summary>
        private readonly FileSystemLoggerConfigureOptions _config;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="queue"></param>
        /// <param name="config"></param>
        public FileSystemLogger(string name, FileSystemLoggerQueue queue, FileSystemLoggerConfigureOptions config)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            CategoryName = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return FileSystemLoggerScope.Scope;
        }

        /// <summary>
        /// 是否启用此日志等级
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= _config.MinLogLevel;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var file = $"{CategoryName}\\{DateTime.Now:yyyy\\\\MM\\\\dd}.{_config.Extension}";
                _queue.AddMessage(file, FormatMessage(logLevel, state.ToString(), exception));
            }
        }

        internal void ToFile(string dateFolder, string fileName, string message, Exception exception, bool isNewFile = false)
        {
            var level = exception == null ? LogLevel.Information : LogLevel.Error;
            var filePath = $"{CategoryName}\\{dateFolder}\\{fileName}.{_config.Extension}";
            _queue.AddMessage(filePath, FormatMessage(level, message, exception), isNewFile);
        }

        /// <summary>
        /// 格式化日志消息
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        private string FormatMessage(LogLevel logLevel, string message, Exception exception)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"->{DateTime.Now:yyyy-MM-dd HH:mm:ss} {logLevel} ");
            if(!string.IsNullOrEmpty(message))stringBuilder.AppendLine($"{message}");
            if (exception != null) stringBuilder.AppendLine(exception.ToString());
            return stringBuilder.ToString();
        }
    }
}
