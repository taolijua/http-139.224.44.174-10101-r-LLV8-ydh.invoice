using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using YDH.Finace.Common;

namespace YDH.Finace.Web.Authentication.YDHAuth
{
    /// <summary>
    /// YDHAuth认证设置
    /// </summary>
    public sealed class YDHAuthOptions : AuthenticationSchemeOptions
    {
        #region const
        /// <summary>
        /// YDHAuth认证方案名称
        /// </summary>
        public const string SchemeName = "YDHAuth";
        /// <summary>
        /// 基础用户(例如开放API用户)
        /// </summary>
        public const string BasicUser = "BasicUser";
        /// <summary>
        /// 系统用户
        /// </summary>
        public const string SystemUser = "SystemUser";
        /// <summary>
        /// 开放的工作操作人员
        /// </summary>
        public const string WorkOperator = "WorkOperator";

        /// <summary>
        /// YDHAuth token声明类型
        /// </summary>
        public const string TokenClaimType = "YDHAuthTokenType";
        /// <summary>
        /// YDHAuth userId声明类型
        /// </summary>
        public const string UserIdClaimType = "YDHAuthUserIdType";
        /// <summary>
        /// 默认名称声明，原则上ClaimsIdentity构造时不允许替换NameClaimType
        /// </summary>
        public const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        /// <summary>
        /// 默认token名称
        /// </summary>
        public const string DefaultTokenName = "YDHAuthToken";
        /// <summary>
        /// 默认OpenApi认证前辍
        /// </summary>
        public const string DefaultOpenApiAuthPrefix = "OpenApi=";
        #endregion

        /// <summary>
        /// YDHAuth认证票据仓库
        /// <para>必须的设施</para>
        /// </summary>
        public IYDHAuthTicketStore TicketStore { set; get; }
        /// <summary>
        /// YDHAuth认证令牌的名称
        /// <para>此令牌包含在http request header或者query</para>
        /// </summary>
        public string TokenName { set; get; }
        /// <summary>
        /// OpenApiAuth认证前辍, 无本地用户
        /// </summary>
        public string OpenApiAuthPrefix { set; get; }
        /// <summary>
        /// 后台工作操作者账号
        /// </summary>
        public string WorkOperatorAccount { set; get; }

    }
    /// <summary>
    /// YDHAuth认证后期设置，用来注入
    /// </summary>
    public sealed class PostConfigureYDHAuthOptions : IPostConfigureOptions<YDHAuthOptions>
    {
        /// <summary>
        /// 配置项
        /// </summary>
        private readonly IConfiguration _config;
        /// <summary>
        /// 认证票据仓库
        /// </summary>
        private readonly IYDHAuthTicketStore _ticketStore;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="ticketStore"></param>
        /// <param name="config"></param>
        public PostConfigureYDHAuthOptions(IYDHAuthTicketStore ticketStore, IConfiguration config)
        {
            _config = config;
            _ticketStore = ticketStore;
        }
        /// <summary>
        /// 后处理方法
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        public void PostConfigure(string name, YDHAuthOptions options)
        {
            options.TicketStore ??= _ticketStore;
            options.TokenName ??= GetConfigurValue(nameof(options.TokenName)) ?? YDHAuthOptions.DefaultTokenName;
            options.OpenApiAuthPrefix ??= GetConfigurValue(nameof(options.OpenApiAuthPrefix)) ?? YDHAuthOptions.DefaultOpenApiAuthPrefix;
        }
        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetConfigurValue(string name)
        {
            var value = _config.GetSection($"{YDHAuthOptions.SchemeName}:{name}")?.Value;
            if (value.IsEmpty()) value = null;
            return value;
        }
    }
}
