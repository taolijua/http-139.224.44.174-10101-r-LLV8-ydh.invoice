using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Attributes;

namespace YDH.Finace.Entities.Management
{
    /// <summary>
    /// 报销人员信息
    /// </summary>
    public class SysReimbursementApplicant : Entity
    {
        /// <summary>
        ///报销人ID
        /// </summary>
        [MapTo(FieldName = "id")]
        public int BXId { get; set; }

        /// <summary>
        ///报销人姓名
        /// </summary>
        [MapTo(FieldName = "name")]
        public string  BXName { get; set; }
    }
}
