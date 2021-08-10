using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace YDH.Finace.Common.FileSystemLogging
{
    /// <summary>
    /// 文件系统日志消息队列
    /// </summary>
    internal sealed class FileSystemLoggerQueue
    {
        /// <summary>
        /// 写入日志标记
        /// </summary>
        private int writeFlag = 0;
        /// <summary>
        /// 消息写入对象
        /// </summary>
        private readonly FileSystemLoggerWriter _writer;
        /// <summary>
        /// 文件系统日志配置选项
        /// </summary>
        private readonly FileSystemLoggerConfigureOptions _config;
        /// <summary>
        /// 消息队列
        /// </summary>
        private readonly ConcurrentQueue<FileSystemLoggerMessage> _concurrentQueue;
        
        
        /// <summary>
        /// 由<see cref="FileSystemLoggerProvider"/>初始化时建立
        /// </summary>
        /// <param name="config"></param>
        public FileSystemLoggerQueue(FileSystemLoggerConfigureOptions config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _concurrentQueue = new ConcurrentQueue<FileSystemLoggerMessage>();
            _writer = new FileSystemLoggerWriter(_config);
        }

        /// <summary>
        /// 添加日志消息
        /// </summary>
        /// <param name="file"></param>
        /// <param name="content"></param>
        /// <param name="isNewFile"></param>
        public void AddMessage(string file, string content, bool isNewFile = false)
        {
            AddMessage(new FileSystemLoggerMessage { File = file, Content = content, IsNewFile = isNewFile });
        }

        /// <summary>
        /// 添加日志消息
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(FileSystemLoggerMessage message)
        {
            _concurrentQueue.Enqueue(message);
            EnqueueNotify();
        }

        /// <summary>
        /// 入队通知
        /// </summary>
        private void EnqueueNotify()
        {
            // 检查是否存在写入线程，不存在则从线程池启动一个
            if (Interlocked.CompareExchange(ref writeFlag, 1, 0) == 0)
            {
                Task.Factory.StartNew(EnqueueNotifyAsync);
            }
        }

        /// <summary>
        /// 入队通知
        /// </summary>
        private async Task EnqueueNotifyAsync()
        {
            try
            {
                await Task.Delay(_config.QueueConsumeDelay);
                HashSet<int> exceptionCounter = null;
                while (true)
                {
                    if (!_concurrentQueue.TryDequeue(out var message)) break;
                    if (message != null && !message.IsEmpty)
                    {
                        try
                        {
                            await _writer.WriteMessageAsync(message);
                        }
                        catch (Exception ex)
                        {
                            if (exceptionCounter == null) exceptionCounter = new HashSet<int>();

                            var exceptionId = message.ExceptionId > 0 ? message.ExceptionId : exceptionCounter.Count + 1;

                            if (exceptionCounter.Add(exceptionId))
                            {
                                var exceptionMessage = new FileSystemLoggerMessage
                                {
                                    File = message.File,
                                    Content = $"{nameof(EnqueueNotifyAsync)} Error:\r\n{message.Content}\r\n{ex.ToString()}"
                                };
                                _concurrentQueue.Enqueue(exceptionMessage);
                            }
                        }
                    }
                }
            }
            finally
            {
                // 释放临界标志为0
                writeFlag = 0;
                // 队列中存在消息则激活新任务
                if (_concurrentQueue.Count > 0) AddMessage(null);
            }
        }
    }
}
