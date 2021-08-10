using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using YDH.Finace.Web.Authentication.YDHAuth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YDH.Finace.Common;
using System.Diagnostics;

namespace YDH.Finace.Web.LimitResource
{
    /// <summary>
    /// 限制请求中间件
    /// </summary>
    public sealed class LimitRequestMiddleware
    {
        private readonly bool _enable;
        private readonly bool _global;
        private readonly string _tokenName;
        private readonly RequestDelegate _next;
        private readonly MemoryCache _semaphores;
        private readonly Dictionary<string, LimitRequestItem> _optionsMap;
        
        public LimitRequestMiddleware(
            RequestDelegate next, 
            IOptions<LimitResourceOptions> options, 
            IOptions<YDHAuthOptions> authOptions)
        {
            _next = next;
            _tokenName = authOptions.Value.TokenName;

            var list = options.Value?.LimitRequestList;
            _enable = (list?.Count ?? 0) > 0;
            if ((list?.Count ?? 0) > 0)
            {
                _enable = true;
                var global = list.FirstOrDefault(i => i.Path == "*");
                if (global != null)
                {
                    _global = true;
                    _optionsMap = new Dictionary<string, LimitRequestItem> { { "*", global } };
                }
                else
                {
                    _optionsMap = list.ToDictionary(i => i.Path, i => i);
                }
                _semaphores = new MemoryCache(new MemoryCacheOptions());
            }
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var semaphore = GetSemaphore(context);
            if (semaphore != null)
            {
                var through = await semaphore.WaitAsync();
                if (through)
                {
                    //Trace.WriteLine($"Through: {context.Request.Path}");
                    try
                    {
                        await _next(context);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                else
                {
                    Trace.WriteLine($"Unavailable: {context.Request.Path}");
                    // 信号获取不成功返回http 503
                    context.Response.StatusCode = 503;
                    await context.Response.WriteAsync(string.Empty);
                }
                return;
            }

            await _next(context);
        }

        /// <summary>
        /// 获取当前请求对应的信号项
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private SemaphoreItem GetSemaphore(HttpContext context)
        {
            var option = GetOption(context);
            if (option != null)
            {
                var key = option.Path;
                if (option.Client)
                {
                    var clientId = GetClientId(context);
                    key = $"{key}_{clientId}";
                }
                if (!_semaphores.TryGetValue(key, out SemaphoreItem semaphore))
                {
                    semaphore = new SemaphoreItem(option.MaxCount);
                    var options = new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromSeconds(600) };
                    _semaphores.Set(key, semaphore, options);
                }
                return semaphore;
            }
            return null;
        }

        /// <summary>
        /// 获取选项
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private LimitRequestItem GetOption(HttpContext context)
        {
            if (!_enable) return null;
            if (_global) return _optionsMap.First().Value;

            var path = context.Request.Path.Value;
            return _optionsMap.FirstOrDefault(i => path.StartsWith(i.Key, StringComparison.OrdinalIgnoreCase)).Value;
        }

        /// <summary>
        /// 获取客户端ID
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private int GetClientId(HttpContext context)
        {
            var clientToken = context.Request.GetToken(_tokenName);
            if (clientToken.HasValue()) return clientToken.GetHashCode();
            return context.Connection.RemoteIpAddress.GetHashCode();
        }

        /// <summary>
        /// 信号量项
        /// </summary>
        private class SemaphoreItem : IDisposable
        {
            /// <summary>
            /// 同步锁
            /// </summary>
            private object syncroot;
            /// <summary>
            /// 简单信号量
            /// </summary>
            private SemaphoreSlim semaphore;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="maxCount"></param>
            public SemaphoreItem(int maxCount)
            {
                semaphore = new SemaphoreSlim(maxCount, maxCount);
                syncroot = new object();
            }
            /// <summary>
            /// 获取一个信号
            /// </summary>
            /// <returns></returns>
            public async ValueTask<bool> WaitAsync()
            {
                lock (syncroot)
                {
                    if (semaphore.CurrentCount <= 0) return false;
                }
                return await semaphore.WaitAsync(20);
            }
            /// <summary>
            /// 释放一个信号
            /// </summary>
            public void Release()
            {
                semaphore.Release();
            }
            /// <summary>
            /// 释放此对象
            /// </summary>
            public void Dispose()
            {
                semaphore?.Dispose();
                semaphore = null;
                syncroot = null;
            }
        }
    }
}
