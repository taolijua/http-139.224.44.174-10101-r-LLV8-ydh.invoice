using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using YDH.Finace.Web.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Web.LimitResource
{
    /// <summary>
    /// 限制动态资源中间件
    /// </summary>
    public class LimitResourceMiddleware
    {
        private readonly LimitResourceOptions _options;
        private readonly RequestDelegate _next;
        public LimitResourceMiddleware(IOptions<LimitResourceOptions> options, RequestDelegate next)
        {
            _options = options?.Value;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(HttpContext));
            // 存在获取终结点操作，所以此中间件应在IApplicationBuilder.UseRouting()之后
            var endpoint = context.GetEndpoint();
            var ipLimitData = endpoint?.Metadata.GetOrderedMetadata<ILimitIPAddressData>() ?? Array.Empty<ILimitIPAddressData>();
            var passThrough = CheckIPLimit(ipLimitData, context.Connection.RemoteIpAddress.ToString());
            if (!passThrough)
            {
                context.Response.StatusCode = 403;
                await Task.CompletedTask;
            }
            await _next(context);
        }

        /// <summary>
        /// 检查IP限制，True表示不限制
        /// </summary>
        /// <param name="ipLimitData"></param>
        /// <param name="ipaddr"></param>
        /// <returns></returns>
        private bool CheckIPLimit(IReadOnlyList<ILimitIPAddressData> ipLimitData, string ipaddr)
        {
            var result = true;
            ipaddr ??= string.Empty;
            foreach (var iplimit in ipLimitData)
            {
                var key = iplimit.IPListName ?? string.Empty;
                if (_options.IPAddressMap.ContainsKey(key))
                {
                    var map = _options.IPAddressMap[key];
                    var contains = map.Contains("*") || map.Contains(ipaddr);
                    result = iplimit.IsWhiteList ? contains : !contains;
                    break;
                }
            }
            return result;
        }
    }
}
