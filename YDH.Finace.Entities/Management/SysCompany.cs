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
    /// 报销公司信息
    /// </summary>
    public  class SysCompany : Entity
    {
        /// <summary>
        ///报销公司ID
        /// </summary>
        [MapTo(FieldName = "id")]
        public int CompanyId { get; set; }

        /// <summary>
        ///报销公司姓名
        /// </summary>
        [MapTo(FieldName = "company_name")]
        public string CompanyName { get; set; }


        /// <summary>
        ///纳税人识别号
        /// </summary>
        [MapTo(FieldName = "identification_number")]
        public string IdentificationNumber { get; set; }
    }
}
