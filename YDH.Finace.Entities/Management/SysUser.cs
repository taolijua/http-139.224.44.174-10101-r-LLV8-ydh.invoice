using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Attributes;

namespace YDH.Finace.Entities.Management
{
    public  class SysUser : Entity
    {
        /// <summary>
        ///ID
        /// </summary>
        [MapTo(FieldName = "id")]
        public int UserId { get; set; }

        /// <summary>
        /// 用户类型ID
        /// </summary>
        [MapTo(FieldName = "type_id")]
        public int  UserType { get; set; }

        /// <summary>
        /// 用户类型名称
        /// </summary>
        [MapTo(FieldName = "user_type")]
        public string  UserTypeName { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        [MapTo(FieldName = "user_status")]
        public int UserStatus { get; set; }


        /// <summary>
        /// 用户账号
        /// </summary>
        [MapTo(FieldName = "user_name")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码MD5
        /// </summary>
   
        [MapTo(FieldName = "password")]
        public string Password { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        [MapTo(FieldName = "token_key")]
        public string TokenKey { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [MapTo(FieldName = "nickname")]
        public string NickName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [MapTo(FieldName = "create_by")]
        public string  CreateBy { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [MapTo(FieldName = "createtime")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 最后修改人
        /// </summary>
        [MapTo(FieldName = "last_modify_by")]
        public string LastModifyBy { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [MapTo(FieldName = "last_modify_time")]
        public DateTime? LastModifyTime { get; set; }
    }
}
