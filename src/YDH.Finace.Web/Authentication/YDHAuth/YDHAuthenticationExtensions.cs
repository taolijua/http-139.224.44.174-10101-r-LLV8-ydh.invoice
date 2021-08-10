using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace YDH.Finace.Web.Authentication.YDHAuth
{
    public static class YDHAuthenticationExtension
    {
        /// <summary>
        /// 为令牌加密
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string TokenEncryption(this string token)
        {
            try
            {
                if (token.IsEmpty()) return string.Empty;
                var array = token.PadRight(32).ToBytes();
                var bytes = array.ToList().ConvertAll(i => (byte)(i >> 4 | i << 4)).ToArray();
                return bytes.ToBase64String();
            }
            catch { return string.Empty; }
        }
        /// <summary>
        /// 为令牌解密
        /// </summary>
        /// <param name="base64Token"></param>
        /// <returns></returns>
        public static string TokenDecryption(this string base64Token)
        {
            try
            {
                var bytes = base64Token.Base64ToBytes().ToList().ConvertAll(i => (byte)(i << 4 | i >> 4)).ToArray();
                return bytes.GetString().Trim();
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 32位加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Get32Md5(this string str)
        {
            byte[] result = Encoding.UTF8.GetBytes(str);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }
    }
}



namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// YDHAuth认证扩展
    /// </summary>
    public static class YDHAuthenticationExtension
    {
        /// <summary>
        /// 附加YDHAuth认证
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddYDHAuth(this AuthenticationBuilder builder, IConfiguration configuration)
        {
            var schemeName = YDHAuthOptions.SchemeName;
            // 认证票据仓库注册为单例，避免多个YDHAuthOptions存在
            builder.Services.AddSingleton<IYDHAuthTicketStore, YDHAuthenticationTicketStore>();
            // 绑定配置
            builder.Services.Configure<YDHAuthOptions>(configuration.GetSection(YDHAuthOptions.SchemeName));
            // 后期配置YDHAuthOptions，配置文件也由此传入
            builder.Services.AddSingleton<IPostConfigureOptions<YDHAuthOptions>, PostConfigureYDHAuthOptions>();
            // 检查认证配置，如果默认方案为空，则使用当前方案
            builder.Services.Configure<AuthenticationOptions>(options => options.DefaultScheme ??= schemeName);
            // 添加方案及配置
            builder.AddScheme<YDHAuthOptions, YDHAuthenticationHandler>(schemeName, null, null);
            return builder;
        }

        /// <summary>
        /// 获取YDHAuthOptions配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal static YDHAuthOptions GetYDHAuthOptions(this IConfiguration configuration)
        {
            if (YDHAuthOptions.IsNull())
            {
                var options = configuration.GetSection(YDHAuthOptions.SchemeName).Get<YDHAuthOptions>();
                var postConfigure = new PostConfigureYDHAuthOptions(null, configuration);
                postConfigure.PostConfigure(null, options);
                YDHAuthOptions = options;
            }
            return YDHAuthOptions;
        }
        private static YDHAuthOptions YDHAuthOptions = null;

        /// <summary>
        /// 添加YDHAuth认证
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddYDHAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddAuthentication().AddYDHAuth(configuration);
        }
    }
}

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// 
    /// </summary>
    public static class YDHAuthenticationExtensions
    {
        public static async Task<string> YDHAuthSignInAsync(this HttpContext context, string userName, long userId, string scheme = YDHAuthOptions.SchemeName, AuthenticationProperties properties = null)
        {
            if (scheme.IsEmpty()) throw new ArgumentNullException(nameof(scheme));
            var identity = new ClaimsIdentity(scheme);
            var principal = new ClaimsPrincipal(identity);
            principal.AddSystemAuthClaim(userName, userId);

            // 1.IAuthenticationService.SignInAsync => AuthenticationService.SignInAsync
            // 2.检查scheme，如果为空使用默认认证方案(IAuthenticationHandler)
            // 3.IAuthenticationHandler.SignInAsync => YDHAuthenticationHandler.SignInAsync
            // 4.生成登录票据并加入缓存，并将缓存的键(token)添加到principal中
            await context.SignInAsync(scheme, principal, properties);
            // 获取上一个操作中为principal添加的token
            var token = principal.GetAuthToken();
            return token;
        }

        /// <summary>
        /// YDHAuth登录
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static async Task YDHAuthSignOutAsync(this HttpContext context, string scheme)
        {
            await context.SignOutAsync(scheme);
        }

        /// <summary>
        /// 获取令牌
        /// </summary>
        /// <param name="request"></param>
        /// <param name="tokenName"></param>
        /// <returns></returns>
        public static string GetToken(this HttpRequest request, string tokenName)
        {
            if (request == null) return null;
            if (!request.Headers.TryGetValue(tokenName, out var token))
            {
                request.Query.TryGetValue(tokenName, out token);
            }
            return token;
        }
    }
}

namespace System.Security.Claims
{
    /// <summary>
    /// ClaimsPrincipal扩展方法
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        #region Add
        /// <summary>
        /// 添加系统认证声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="identityName"></param>
        /// <param name="userId"></param>
        public static void AddSystemAuthClaim(this ClaimsPrincipal user, string identityName, long userId)
        {
            user.AddAuthClaim(identityName, null, userId);
        }
        /// <summary>
        /// 添加基础认证声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="identityName"></param>
        /// <param name="token"></param>
        public static void AddBasicAuthInfo(this ClaimsPrincipal user, string identityName, string token)
        {
            user.AddAuthClaim(identityName, token, 0);
        }
        /// <summary>
        /// 添加认证声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="name"></param>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        private static void AddAuthClaim(this ClaimsPrincipal user, string name, string token, long userId)
        {
            if (token.HasValue()) user.AddTokenClaim(token);

            if (name.HasValue()) 
                user.AddClaim(new Claim(YDHAuthOptions.NameClaimType, name));
            
            if(userId > 0)
                user.AddClaim(new Claim(YDHAuthOptions.UserIdClaimType, userId.ToString()));
        }
        /// <summary>
        /// 添加Token声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        public static void AddTokenClaim(this ClaimsPrincipal user, string token)
        {
            user.AddClaim(new Claim(YDHAuthOptions.TokenClaimType, token));
        }
        /// <summary>
        /// 添加声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        public static void AddClaim(this ClaimsPrincipal user, Claim claim)
        {
            if (claim == null) throw new ArgumentNullException(nameof(claim));
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.GetIdentity().AddClaim(claim);
        }
        #endregion

        #region Get
        /// <summary>
        /// 获取认证的令牌
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetAuthToken(this ClaimsPrincipal user)
        {
            return user.FirstClaim(YDHAuthOptions.TokenClaimType)?.Value;
        }
        /// <summary>
        /// 获取认证的用户ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static long GetAuthUserId(this ClaimsPrincipal user)
        {
            return user.FirstClaim(YDHAuthOptions.UserIdClaimType)?.Value?.ToInt64() ?? 0;
        }
        /// <summary>
        /// 获取首个Claim
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static Claim FirstClaim(this ClaimsPrincipal user, string claimType)
        {
            if (user == null) return null;
            if (claimType.IsEmpty()) return null;
            return user.FindFirst(claimType);
        }
        /// <summary>
        /// 获取Identity
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static ClaimsIdentity GetIdentity(this ClaimsPrincipal user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.Identity as ClaimsIdentity ?? throw new ArgumentException(nameof(ClaimsPrincipal.Identity));
        }
        #endregion

        #region Has
        /// <summary>
        /// 是否存在 Identity.Name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool HasIdentityName(this ClaimsPrincipal user)
        {
            return !string.IsNullOrEmpty(user?.GetIdentity()?.Name);
        }
        /// <summary>
        /// 包括任一claimType
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimTypes"></param>
        /// <returns></returns>
        public static bool AnyClaim(this ClaimsPrincipal user, params string[] claimTypes)
        {
            if (claimTypes.Length == 0) return false;
            return user?.HasClaim(claim => claimTypes.Contains(claim.Type)) ?? false;
        }
        /// <summary>
        /// 包括所有claimType
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimTypes"></param>
        /// <returns></returns>
        public static bool AllClaim(this ClaimsPrincipal user, params string[] claimTypes)
        {
            return claimTypes?.All(claimType => user.HasClaim(claim => claim.Type.Equals(claimType))) ?? false;
        }
        #endregion

        #region Remove
        /// <summary>
        /// 移除认证声明
        /// </summary>
        /// <param name="user"></param>
        public static void RemoveAuthClaim(this ClaimsPrincipal user)
        {
            var types = new string[] 
            { 
                YDHAuthOptions.NameClaimType, 
                YDHAuthOptions.TokenClaimType, 
                YDHAuthOptions.UserIdClaimType
            };
            user.RemoveClaims(types);
        }
        /// <summary>
        /// 移除声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claimTypes"></param>
        public static void RemoveClaims(this ClaimsPrincipal user, params string[] claimTypes)
        {
            if (user == null) return;
            if (claimTypes.Length == 0) return;
            foreach (var identity in user.Identities)
            {
                foreach (var claim in identity.FindAll(c => claimTypes.Contains(c.Type)).ToList())
                {
                    identity.TryRemoveClaim(claim);
                }
            }
        }
        #endregion
    }
}



