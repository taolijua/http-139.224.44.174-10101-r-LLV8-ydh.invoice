using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;

namespace YDH.Finace.Entities.Dto
{
   public class LoginDto : Model
    {
        /// <summary>
        ///  用户账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string TimeStamp { get; set; }

        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt { get; set; }

    }
}
