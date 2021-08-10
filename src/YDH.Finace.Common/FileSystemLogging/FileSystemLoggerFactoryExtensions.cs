using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YDH.Finace.Common.FileSystemLogging;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 文件系统日志扩展
    /// </summary>
    public static class FileSystemLoggerFactoryExtensions
    {
        /// <summary>
        /// 添加文件系统日志
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection AddFileSystemLogging(this IServiceCollection services, IConfiguration config)
        {
            return services.AddLogging(builder =>
            {
                var loggingSection = config.GetSection(nameof(YDH.Finace.Common.FileSystemLogging));
                builder.Services.Configure<FileSystemLoggerConfigureOptions>(loggingSection);
                builder.Services.AddSingleton<ILoggerProvider, FileSystemLoggerProvider>();
            });
        }
    }
}

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// 文件系统日志扩展方法
    /// </summary>
    public static class FileSystemLoggerFactoryExtensions
    {
        /// <summary>
        /// 日志写入到新文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void NewFile<T>(this ILogger<T> logger, string fileName, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy\\\\MM\\\\dd}", fileName, message, ex, true);
        }
        /// <summary>
        /// 日志写入到新文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="fileName"></param>
        /// <param name="ex"></param>
        public static void NewFile<T>(this ILogger<T> logger, string fileName, Exception ex)
        {
            logger.NewFile(fileName, string.Empty, ex);
        }


        /// <summary>
        /// 日志追加到指定文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void AppendFile<T>(this ILogger<T> logger, string fileName, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy\\\\MM\\\\dd}", fileName, message, ex);
        }
        /// <summary>
        /// 日志追加到指定文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="fileName"></param>
        /// <param name="ex"></param>
        public static void AppendFile<T>(this ILogger<T> logger, string fileName, Exception ex)
        {
            logger.AppendFile(fileName, string.Empty, ex);
        }


        /// <summary>
        /// 日志按每小时一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void HourlyRollFile<T>(this ILogger<T> logger, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy\\\\MM\\\\dd}", $"{DateTime.Now:HH}", message, ex);
        }
        /// <summary>
        /// 日志按每小时一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void HourlyRollFile<T>(this ILogger<T> logger, Exception ex)
        {
            logger.HourlyRollFile(null, ex);
        }


        /// <summary>
        /// 日志按每天一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void DailyRollFile<T>(this ILogger<T> logger, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy\\\\MM}", $"{DateTime.Now:dd}", message, ex);
        }
        /// <summary>
        /// 日志按每天一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void DailyRollFile<T>(this ILogger<T> logger, Exception ex)
        {
            logger.DailyRollFile(null, ex);
        }


        /// <summary>
        /// 日志按每周一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WeeklyRollFile<T>(this ILogger<T> logger, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy\\\\MM}", $"{ DateTime.Now.DayOfWeek}", message, ex);
        }
        /// <summary>
        /// 日志按每周一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void WeeklyRollFile<T>(this ILogger<T> logger, Exception ex)
        {
            logger.WeeklyRollFile(null, ex);
        }


        /// <summary>
        /// 日志按每月一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void MonthlyRollFile<T>(this ILogger<T> logger, string message, Exception ex = null)
        {
            FileSystemLoggerProvider.GetLogger<T>()?.ToFile($"{DateTime.Now:yyyy}", $"{ DateTime.Now:MM}", message, ex);
        }
        /// <summary>
        /// 日志按每月一个文件滚动
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ex"></param>
        public static void MonthlyRollFile<T>(this ILogger<T> logger, Exception ex)
        {
            logger.MonthlyRollFile(null, ex);
        }

        /// <summary>
        /// 添加文件系统日志
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        public static void AddFileSystemLogger(this ILoggingBuilder builder, IConfiguration config)
        {
            builder.Services.Configure<FileSystemLoggerConfigureOptions>(config.GetSection(nameof(YDH.Finace.Common.FileSystemLogging)));
            builder.Services.AddSingleton<ILoggerProvider, FileSystemLoggerProvider>();
        }
    }
}
