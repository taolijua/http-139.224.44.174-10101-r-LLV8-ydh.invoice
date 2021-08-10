using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common.AllExceptions;
using YDH.Finace.Common.BusinessServices;
using YDH.Finace.DBContext.Repositories.Management;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;
using YDH.Finace.Web.FilterAttributes;

namespace YDH.Finace.Business.Management
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : BusinessService, IUserService
    {  // 日记
        private readonly ILogger<LoginService> _logger = null;
        // 获取依赖注入
        private readonly IServiceScope scope = null;
        public UserService(IServiceProvider serviceProvider, ILogger<LoginService> logger)
        {
            scope = serviceProvider.CreateScope();
            _logger = logger;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [SkipResultFilter]
        public async Task<ResponseRep<SysUser>> QueryUserListAsync(PageRequestDto<SysUser> entity) 
        {
            if (entity == null)
            {
                throw new FriendlyException("参数不能为空！");
            }
            if (entity.limit < 1) entity.limit = 50;// 一页默认50行

            // 权限管理实例
            var managementRepository = scope.ServiceProvider.GetRequiredService<IManagementRepository>();

            //
            return await managementRepository.QueryUserListAsync(entity);
        }
        /// <summary>
        /// 增加用户
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysUser>> AddUserAsync(SysUser UserInfo)
        {
            if (UserInfo == null)
            {
                throw new FriendlyException("参数不能为空！");
            }
            // 权限管理实例
            var managementRepository = scope.ServiceProvider.GetRequiredService<IManagementRepository>();
            
            return await managementRepository.InsertUserAsync(UserInfo); 

        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysUser>> ModifyUserAsync(SysUser UserInfo)
        {
            //验证参数
            if (UserInfo == null)
            {
                throw new FriendlyException("参数不能为空！");
            }
    
            // 权限管理实例
            var managementRepository = scope.ServiceProvider.GetRequiredService<IManagementRepository>();
        
            return await managementRepository.ModifyUserAsync(UserInfo);
        }
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        public async Task<int> DeleteUserAsync(SysUser UserInfo)
        {
            //验证参数
            if (UserInfo == null)
            {
                throw new FriendlyException("参数不能为空！");
            }
            // 权限管理实例
            var managementRepository = scope.ServiceProvider.GetRequiredService<IManagementRepository>();
            //验证用户是否存在
            SysUser User = await managementRepository.GetUserAsync(UserInfo.UserName);
            if (User == null)
            {
                throw new FriendlyException("该用户不存在，请输入正确的用户账号！");
            }
            return await managementRepository.DeleteUserAsync(UserInfo);
        }
    }

    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IUserService : IBusinessService
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysUser>> QueryUserListAsync(PageRequestDto<SysUser> entity);

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysUser>> AddUserAsync(SysUser UserInfo);


        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysUser>> ModifyUserAsync(SysUser UserInfo);

        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        Task<int> DeleteUserAsync(SysUser UserInfo);
    }
}
