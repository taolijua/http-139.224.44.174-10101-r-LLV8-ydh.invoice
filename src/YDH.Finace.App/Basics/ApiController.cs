using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YDH.Finace.Web.Authentication.YDHAuth;
using YDH.Finace.Web.FilterAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace YDH.Finace.App.Basics
{
    /// <summary>
    /// WebApi控制器基类
    /// <para>使用基于经典路由模板的属性路由(登录用户)</para>
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultFilter, ApiExceptionFilter]
    [Authorize(Policy = YDHAuthOptions.SystemUser)]
    public abstract class ApiController : ControllerBase
    {
        protected string UserName => User.Identity.Name;
    }

    /// <summary>
    /// WebApi控制器基类
    /// <para>基础用户(例如开放API用户)</para>
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultFilter, ApiExceptionFilter]
    [Authorize(Policy = YDHAuthOptions.BasicUser)]
    public abstract class BasicController : ControllerBase
    {
     
    }



    /// <summary>
    /// Rest风格的WebApi控制器基类
    /// <para>action模板缺省使用http 动词</para>
    /// </summary>
    [Route("api/[controller]")]
    public abstract class RestController : ApiController
    {
        
    }

    /// <summary>
    /// 开放式Api
    /// <para>使用基于经典路由模板的属性路由</para>
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ApiResultFilter, ApiExceptionFilter]
    public abstract class OpenController : ControllerBase
    {

    }
}
