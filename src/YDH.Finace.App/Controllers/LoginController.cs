using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YDH.Finace.App.Basics;
using YDH.Finace.Business.Management;
using YDH.Finace.Entities.Dto;
using YDH.Finace.Entities.Management;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace YDH.Finace.App.Controllers
{
    /// <summary>
    /// 登录入口,开放式api
    /// </summary>
    public class LoginController : OpenController
    {
        private readonly ILoginService _loginService = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginService"></param>
        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public async Task<SysUser> LoginAsync(LoginDto dto)
        {
            return await _loginService.LoginAsync(dto);
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public async Task SignOutAsync()
        {
            await HttpContext.YDHAuthSignOutAsync(YDHAuthOptions.SchemeName);
        }


        /// <summary>
        /// 这个接口不能上正式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = YDHAuthOptions.SystemUser)]
        public string LoginPlus([FromBody]string str)
        {
            #if DEBUG
            str = str.Get32Md5();
            #endif
            return str;
        }
    }
}
