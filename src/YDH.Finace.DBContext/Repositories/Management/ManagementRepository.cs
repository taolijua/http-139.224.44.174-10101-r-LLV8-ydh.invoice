
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Interfaces;
using YDH.Finace.Data.Repositories;
using YDH.Finace.Entities.Base;
using YDH.Finace.Entities.Management;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace YDH.Finace.DBContext.Repositories.Management
{
    /// <summary>
    /// 财务报销权限管理
    /// </summary>
    public class ManagementRepository : Repository, IManagementRepository
    {

        private readonly ILogger<ManagementRepository> _logger = null;
        private readonly IMySqlContext _mySql = null;
        // 获取依赖注入
        private readonly IServiceScope scope = null;
        public ManagementRepository(IServiceProvider serviceProvider, ILogger<ManagementRepository> logger, IMySqlContext mySql)
        {
            scope = serviceProvider.CreateScope();
            _logger = logger;
            _mySql = mySql;
        }

        #region 用户管理
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysUser>> InsertUserAsync(SysUser entity)
        {
            ReturnDataDto<SysUser> rt = new ReturnDataDto<SysUser>();
            string sql = "INSERT INTO cw_sys_user (  " +
                         "  type_id " +
                         "  ,username " +
                         "  ,password " +
                         "  ,user_status" +
                         "  ,nickname  " +
                          "  ,create_by " +
                         "  ,createtime ) " +
                         "VALUES (  " +
                         "   @type_Id" +
                         "  ,@username" +
                         "  ,@password" +
                         "  ,@userStatus" +
                         "  ,@nickName" +
                         "  ,@createBy" +
                         "  ,@createtime);";
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            long userid = context.HttpContext.User.GetAuthUserId();
            string password = entity.Password.Get32Md5();

            var @params = new MySqlParameter[]
            {
                new MySqlParameter("type_Id", entity.UserType.ToString()),
                new MySqlParameter("username", entity.UserName),
                new MySqlParameter("password",password  ),
                new MySqlParameter("userStatus", entity.UserStatus.ToString()),
                new MySqlParameter("nickName", entity.NickName),
                new MySqlParameter("createBy", userid.ToString()),
                new MySqlParameter("createtime", DateTime.Now.ToString())
            };
             await _mySql.ExecuteNonQueryAsync(sql, @params);
            rt.Code = 1;
            rt.Message = "添加用户成功";
            return rt;
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ReturnDataDto<SysUser>> ModifyUserAsync(SysUser entity)
        {
            ReturnDataDto<SysUser> rt=new ReturnDataDto<SysUser>();
            string sql = "UPDATE " +
                      " cw_sys_user " +
                     "  set password = @password, " +
                     "  last_modify_by = @lastModifyBy, " +
                     "  last_modify_time = @lastModifyTime " +
                     "  where " +
                     "  id = @ID";
            var context = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            long userid = context.HttpContext.User.GetAuthUserId();
            string password = entity.Password.Get32Md5();

            var @params = new MySqlParameter[]
                {
                    new MySqlParameter("ID",userid),
                    new MySqlParameter("password",entity.Password.Get32Md5()),
                    new MySqlParameter("lastModifyTime",DateTime.Now),
                    new MySqlParameter("lastModifyBy",userid)
                };
            await _mySql.ExecuteNonQueryAsync(sql, @params);
            rt.Code = 1;
            rt.Message = "密码修改成功";
            return rt;
        }
        /// <summary>
        /// 删除用户信息（伪删除，将状态码改为1）
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> DeleteUserAsync(SysUser entity)
        {
            string sql = "UPDATE " +
                      " cw_sys_user " +
                     "set user_status = 1 " +
                     "where " +
                     " username = @username";
            var @params = new MySqlParameter[]
                {  
                    //new MySqlParameter("user_code",entity.UserCode)
                };
            return await _mySql.ExecuteNonQueryAsync(sql, @params);
        }

        /// <summary>
        /// 通过账号查询用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SysUser> GetUserAsync(string username)
        {
            var sql = "select " +
                "       *" +
                "      from " +
                "      cw_sys_user " +
                "      where " +
                "      username = @username;";
            var @params = new MySqlParameter[]
            {
                new MySqlParameter("username", username)
            };
            return await _mySql.EntityAsync<SysUser>(sql, @params);
        }


        /// <summary>
        /// 查询用户信息列表
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="count"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<ResponseRep<SysUser>> QueryUserListAsync(PageRequestDto<SysUser> entity)
        {
            ResponseRep<SysUser> pd = new ResponseRep<SysUser>();

            string where = " 1=1 and user_status = 0  ";

            var @params = new List<MySqlParameter>();

            #region 查询条件

            if (!string.IsNullOrWhiteSpace(entity.Dto?.UserName))
            {
                where += $" and username like @UserName ";
                @params.Add(new MySqlParameter("UserName", entity.Dto?.UserName + '%'));
            }

            if (!string.IsNullOrWhiteSpace(entity.Dto?.NickName))
            {
                where += $" and nickname LIKE @NickName ";
                @params.Add(new MySqlParameter("NickName", entity.Dto?.NickName +'%'));
            }
            #endregion

            var sql = $"select count(*) from cw_sys_user where {where};";

            pd.Count = await _mySql.ExecuteScalarAsync<int>(sql, @params.ToArray() ?? null);

            if (pd.Count < 1) return null;

            sql = "   SELECT cu.*, ct.user_type" +
                  "   FROM cw_sys_user cu " +
                  "  left outer join cw_user_type ct " +
                  "  on cu.type_id = ct.id" +
                  "   WHERE " +
                 $"   {where}    " +
                 $"   limit {(entity.Page == 1 ? 0 : entity.Page) * entity.limit},{entity.limit};";

            pd.Data = await _mySql.ListAsync<SysUser>(sql, @params.ToArray() ?? null);

            return pd;
        }



        #endregion
    }

    /// <summary>
    /// 财务报销权限管理
    /// </summary>
    public interface IManagementRepository : IRepository
    {
        #region 用户管理
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysUser>> InsertUserAsync(SysUser entity);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ReturnDataDto<SysUser>> ModifyUserAsync(SysUser entity);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> DeleteUserAsync(SysUser entity);
        /// <summary>
        /// 通过账号查询用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        Task<SysUser> GetUserAsync(string userCode);

        /// <summary>
        /// 查询用户信息列表
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ResponseRep<SysUser>> QueryUserListAsync(PageRequestDto<SysUser> entity);
        #endregion

    }
}
