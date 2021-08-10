using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志记录器提供程序
    /// </summary>
    internal class FileSystemLoggerProvider : ILoggerProvider
    {
        #region 静态成员
        /// <summary>
        /// 提供程序实例
        /// </summary>
        private static FileSystemLoggerProvider _instance;

        /// <summary>
        /// 获取文件系统日志记录器
        /// <para>仅FileSystemLogger实例</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FileSystemLogger GetLogger<T>()
        {
            if (_instance.IsNull())
            {
                FileSystemLoggerProvider provider = null;
                // 主机访问器可用，通过主机访问器从DI容器获取
                if (HostAccessor.Current.Enabled)
                {
                    provider = HostAccessor.Current.Services
                        .GetServices<ILoggerProvider>()?
                        .FirstOrDefault(i => typeof(FileSystemLoggerProvider) == i.GetType()) as FileSystemLoggerProvider;
                }
                // 最后如果不存在自行构建
                _instance = provider ?? new FileSystemLoggerProvider(new FileSystemLoggerConfigureOptions());
            }
            return _instance.CreateLogger<T>();
        }
        #endregion

        /// <summary>
        /// 文件系统日志队列
        /// </summary>
        private readonly FileSystemLoggerQueue _queue;
        /// <summary>
        /// 文件系统日志配置项
        /// </summary>
        private readonly FileSystemLoggerConfigureOptions _config;
        /// <summary>
        /// 文件系统日志主记录器字典
        /// </summary>
        private readonly ConcurrentDictionary<string, FileSystemLogger> _loggers;

        /// <summary>
        /// 此构造由DI使用
        /// </summary>
        /// <param name="options">使用<see cref="IOptionsMonitor{TOptions}"/>可获取即时更新??</param>
        public FileSystemLoggerProvider(IOptions<FileSystemLoggerConfigureOptions> options) : this(options?.Value) { }
        /// <summary>
        /// 构造提供程序实例
        /// </summary>
        /// <param name="config"></param>
        private FileSystemLoggerProvider(FileSystemLoggerConfigureOptions config)
        {
            _instance ??= this;
            _config = config?.Extend() ?? throw new ArgumentNullException(nameof(config));
            _queue = new FileSystemLoggerQueue(_config);
            _loggers = new ConcurrentDictionary<string, FileSystemLogger>();
        }
        /// <summary>
        /// 创建指定类别名称的文件系统日志记录器
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return GetOrCreate(categoryName);
        }
        /// <summary>
        /// 创建指定类型的文件系统日志记录器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private FileSystemLogger CreateLogger<T>()
        {
            return GetOrCreate(TypeNameHelper.GetTypeDisplayName(typeof(T), true, false, false, '.'));
        }
        /// <summary>
        /// 创建或获取指定类别的文件系统日志记发器
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        private FileSystemLogger GetOrCreate(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new FileSystemLogger(name, _queue, _config));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
