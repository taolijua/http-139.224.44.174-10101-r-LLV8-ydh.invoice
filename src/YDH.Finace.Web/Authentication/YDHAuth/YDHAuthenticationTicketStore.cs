using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.Web.Authentication.YDHAuth
{
    /// <summary>
    /// IYDHAuthTicketStore实现
    /// </summary>
    public sealed class YDHAuthenticationTicketStore : IYDHAuthTicketStore
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 将认证票据加入仓库并返回一个令牌(Key)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var scheme = ticket.AuthenticationScheme;
            var userId = ticket.Principal.GetAuthUserId();
            var srcText = $"{scheme}-{userId}-{Guid.NewGuid():N}";
            var token = srcText.ToBase64String();
            await RenewAsync(token, ticket);
            return token;
        }
        /// <summary>
        /// 添加或更新token的认证票据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Task RenewAsync(string token, AuthenticationTicket ticket)
        {
            var option = new MemoryCacheEntryOptions();
            option.SetSlidingExpiration(TimeSpan.FromHours(12));
            if (ticket.Properties.ExpiresUtc.HasValue)
            {
                option.SetAbsoluteExpiration(ticket.Properties.ExpiresUtc.Value);
            }
            _cache.Set(token, ticket, option);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 根据token获取认证票据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AuthenticationTicket> RetrieveAsync(string token)
        {
            AuthenticationTicket ticket = null;
            if (!string.IsNullOrEmpty(token))
            {
                _cache.TryGetValue(token, out ticket);
            }
            return Task.FromResult(ticket);
        }
        /// <summary>
        /// 删除指定token的认证票据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<AuthenticationTicket> RemoveAsync(string token)
        {
            AuthenticationTicket ticket = null;
            if (!string.IsNullOrEmpty(token) && 
                _cache.TryGetValue(token, out ticket))
            {
                _cache.Remove(token);
                ticket.Principal.RemoveAuthClaim();
            }
            return Task.FromResult(ticket);
        }

    }

    /// <summary>
    /// 类似session stroe
    /// </summary>
    public interface IYDHAuthTicketStore
    {
        /// <summary>
        /// 将认证票据加入仓库并返回一个令牌(Key)
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task<string> StoreAsync(AuthenticationTicket ticket);
        /// <summary>
        /// 添加或更新token的认证票据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        Task RenewAsync(string key, AuthenticationTicket ticket);
        /// <summary>
        /// 根据token获取认证票据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<AuthenticationTicket> RetrieveAsync(string key);
        /// <summary>
        /// 删除指定token的认证票据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<AuthenticationTicket> RemoveAsync(string key);
    }

}
