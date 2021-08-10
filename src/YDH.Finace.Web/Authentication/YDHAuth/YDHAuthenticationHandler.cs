using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.Web.Authentication.YDHAuth
{
    /// <summary>
    /// YDHAuth认证处理方法
    /// <para>
    /// DI容器中<see cref="IAuthenticationService"/>与
    /// <see cref="IAuthenticationHandlerProvider"/>都是基于作用域（请求）的，
    /// 而<see cref="IAuthenticationHandler"/>是瞬态的，但在<see cref="IAuthenticationHandlerProvider"/>中有缓存，所以也是基于请求的
    /// </para>
    /// </summary>
    public sealed class YDHAuthenticationHandler : SignInAuthenticationHandler<YDHAuthOptions>
    {
        /// <summary>
        /// 构建 YDHAuth认证处理方法 的实例
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public YDHAuthenticationHandler(
            IOptionsMonitor<YDHAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock) { }


        /// <summary>
        /// 为当前上下文处理身份认证
        /// <para>基类中已处理了重复读取</para>
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = await YDHAuthTokenResultOnceTask;
            if (!result.Succeeded)return result;

            var validate = await ValidateUserAsync(result.Ticket);
            if (!validate) return AuthenticateResult.Fail("主体认证错误");
            
            return result;
        }
        
        /// <summary>
        /// 签入处理程序
        /// </summary>
        /// <param name="user"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            properties ??= new AuthenticationProperties();
            var ticket = new AuthenticationTicket(user, properties, Scheme.Name);
            _ydhAuthToken = await Options.TicketStore.StoreAsync(ticket);
            ticket.Principal.AddTokenClaim(_ydhAuthToken);
            Logger.LogInformation($"SystemUser({ticket.Principal.Identity.Name}) SignIn");
        }
        /// <summary>
        /// 签出处理程序
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            var ticket = await Options.TicketStore.RemoveAsync(YDHAuthToken);
            Logger.LogInformation($"SystemUser({ticket?.Principal.Identity.Name}) SignOut");
        }

        /// <summary>
        /// 验证用户有效性
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private Task<bool> ValidateUserAsync(AuthenticationTicket ticket)
        {
            var result = ticket.Principal.HasIdentityName();

            //var userId = ticket.Principal.GetYDHAuthUserIdClaimValue();
            //if (userId > 0)
            //{
            // TODO:这里检索缓存或数据库来确定userId有效性
            // var user = User.GetById(userid);
            // ticket.Properties.SetParameter<User>("_YDHAuthUserKey", user);
            // TODO: 可以检查用户权限声明、策略、角色是否变化并且动态更新ticket
            //result = true;
            //}

            //Logger.LogInformation($"Identity.Name:{ticket?.Principal.Identity.Name} Authenticate:{result}");
            return Task.FromResult(result);
        }
        /// <summary>
        /// 获取YDHAuth认证结果
        /// </summary>
        /// <returns></returns>
        private async Task<AuthenticateResult> GetYDHAuthTokenResultAsync()
        {
            var token = YDHAuthToken;
            var ticket = await Options.TicketStore.RetrieveAsync(token);
            if (ticket == null)
            {
                if (token.HasValue() && token.StartsWith(Options.OpenApiAuthPrefix))
                {
                    ticket = await HandleBasicUserSignInAsync(token);
                    if (ticket == null) 
                        return AuthenticateResult.NoResult();
                }
                else
                {
                    return AuthenticateResult.NoResult();
                }
            }
            var currentUtc = Clock.UtcNow;
            if (ticket.Properties.ExpiresUtc.HasValue &&
                ticket.Properties.ExpiresUtc < currentUtc)
            {
                await Options.TicketStore.RemoveAsync(token);
                if (!token.StartsWith(Options.OpenApiAuthPrefix))
                {
                    Logger.LogInformation($"Identity.Name:{ticket.Principal.Identity.Name} Expired");
                    return AuthenticateResult.Fail("认证已过期");
                }
            }
            return AuthenticateResult.Success(ticket);
        }

        /// <summary>
        /// 基本用户签入
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<AuthenticationTicket> HandleBasicUserSignInAsync(string token)
        {
            if (TryGetOpenApiUserName(token, out var userName))
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(Options.TokenName));
                var properties = new AuthenticationProperties { ExpiresUtc = DateTime.UtcNow.AddDays(1) };
                var ticket = new AuthenticationTicket(principal, properties, Scheme.Name);
                await Options.TicketStore.RenewAsync(token, ticket);
                ticket.Principal.AddBasicAuthInfo(userName, token);
                Logger.LogInformation($"OpenUser({userName}) SignIn");
                return ticket;
            }
            return null;
        }

        /// <summary>
        /// 获取OpenApi令牌中的用户名
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private bool TryGetOpenApiUserName(string token, out string userName)
        {
            userName = token.Substring(Options.OpenApiAuthPrefix.Length).TokenDecryption();
            return userName.HasValue();
        }

        /// <summary>
        /// 只获取一次YDHAuth认证结果
        /// </summary>
        private Task<AuthenticateResult> YDHAuthTokenResultOnceTask
        {
            get
            {
                if (_ydhAuthTokenResultOnceTask == null)
                {
                    _ydhAuthTokenResultOnceTask = GetYDHAuthTokenResultAsync();
                }
                return _ydhAuthTokenResultOnceTask;
            }
        }
        private Task<AuthenticateResult> _ydhAuthTokenResultOnceTask;

        /// <summary>
        /// 客户端提供的YDHAuth令牌
        /// </summary>
        private string YDHAuthToken
        {
            get
            {
                if (string.IsNullOrEmpty(_ydhAuthToken))
                {
                    var tokenName = Options.TokenName;
                    _ydhAuthToken = Context.Request.GetToken(tokenName);
                }
                return _ydhAuthToken;
            }
        }
        private string _ydhAuthToken;
    }
}
