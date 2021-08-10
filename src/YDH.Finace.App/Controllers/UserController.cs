using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YDH.Finace.App.Basics;
using YDH.Finace.Business.Management;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;

namespace YDH.Finace.App.Controllers
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public class UserController : ApiController
    {

        private readonly IUserService _userService = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseRep<SysUser>> QueryUserListAsync(PageRequestDto<SysUser> entity)
        {
            return await _userService.QueryUserListAsync(entity);
           
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysUser>> AddUserAsync(SysUser UserInfo)
        {
          return await _userService.AddUserAsync(UserInfo);
        }

        /// <summary>修改用户信息
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ReturnDataDto<SysUser>> ModifyUserAsync(SysUser UserInfo)
        {
            return await _userService.ModifyUserAsync(UserInfo);
        }
        /// <summary>删除用户（伪删除，将状态改为1）
        /// </summary>
        /// <param name="UserInfo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> DeleteUserAsync(SysUser UserInfo)
        {
            return await _userService.DeleteUserAsync(UserInfo);
        }
    }
}
