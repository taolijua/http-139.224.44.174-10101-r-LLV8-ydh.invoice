using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common.AllExceptions;
using YDH.Finace.Common.BusinessServices;
using YDH.Finace.Common.Cache;
using YDH.Finace.DBContext.Repositories.Management;
using YDH.Finace.Entities.Dto;
using YDH.Finace.Entities.Management;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace YDH.Finace.Business.Management
{
    public class LoginService : BusinessService, ILoginService
    {
        // 日记
        private readonly ILogger<LoginService> _logger = null;
        // 获取依赖注入
        private readonly IServiceScope scope = null;
        public LoginService(IServiceProvider serviceProvider, ILogger<LoginService> logger)
        {
            scope = serviceProvider.CreateScope();
            _logger = logger;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<SysUser> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Encrypt) || string.IsNullOrWhiteSpace(dto.TimeStamp))
            {
                throw new FriendlyException("参数不能为空！登录失败!");
            }
            // 权限管理实例
            var managementRepository = scope.ServiceProvider.GetRequiredService<IManagementRepository>();
            // 缓存实例
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
          
            // 重缓存获取，获取用户信息
            SysUser user = await cacheService.GetValueAsync<SysUser>(dto.UserName, async delegate
            {
                // 缓存没有重新到数据库查找添加进缓存里
                return await managementRepository.GetUserAsync(dto.UserName);
            });
           
            // 对比密码是否正确
            if (user!=null && dto.Encrypt.Equals((user.UserName + user.Password + dto.TimeStamp).Get32Md5()))
            {

                // Http上下文
                var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
                // 登录用户绑定上下文
                user.TokenKey  = await context.HttpContext.YDHAuthSignInAsync(user.UserName, user.UserId);
                // 加密返回值
                user.Password = "";
                // 返回对象
                return user;
            }
            throw new FriendlyException("用户或者密码错误！登录失败!");
        }
    }

    /// <summary>
    /// 登录服务
    /// </summary>
    public interface ILoginService : IBusinessService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<SysUser> LoginAsync(LoginDto dto);
    }
}
