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
    /// 发票详情实体
    /// </summary>
    public  class SysInvoice: Entity
    {
        /// <summary>
        ///报销ID
        /// </summary>
        [MapTo(FieldName = "id")]
        public int Id { get; set; }

        /// <summary>
        ///发票代码
        /// </summary>
        [MapTo(FieldName = "invoice_code")]
        public string InvoiceCode { get; set; }

        /// <summary>
        ///发票号码
        /// </summary>
        [MapTo(FieldName = "invoice_number")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        ///开票日期
        /// </summary>
        [MapTo(FieldName = "invoice_date")]
        public DateTime? InvoiceDate { get; set; }

        /// <summary>
        ///开票日期-查询开始日期
        /// </summary>
        public DateTime? InvoiceDateStar { get; set; }

        /// <summary>
        ///开票日期-查询结束日期
        /// </summary>
        public DateTime? InvoiceDateEnd { get; set; }

        /// <summary>
        ///校验码
        /// </summary>
        [MapTo(FieldName = "check_code")]
        public string CheckCode { get; set; }

        /// <summary>
        ///校验码（后6位）
        /// </summary>
        [MapTo(FieldName = "check_code_six")]
        public string CheckCodeSix { get; set; }


        /// <summary>
        ///金额
        /// </summary>
        [MapTo(FieldName = "money")]
        public string Money { get; set; }


        /// <summary>
        ///报销人员
        /// </summary>
        [MapTo(FieldName = "name")]
        public string BXName { get; set; }

        /// <summary>
        ///报销人员id
        /// </summary>
        [MapTo(FieldName = "bx_id")]
        public int? BXId { get; set; }


        /// <summary>
        ///录入时间
        /// </summary>
        [MapTo(FieldName = "input_time")]
        public DateTime? InputTime { get; set; }

        /// <summary>
        ///录入时间-查询开始日期
        /// </summary>
        public DateTime? InputTimeStar { get; set; }

        /// <summary>
        ///录入时间-查询开始日期
        /// </summary>
        public DateTime? InputTimeEnd { get; set; }

        /// <summary>
        ///录入人员
        /// </summary>
        [MapTo(FieldName = "nickname")]
        public string NickName { get; set; }

        /// <summary>
        ///录入人员ID
        /// </summary>
        [MapTo(FieldName = "input_id")]
        public int? InputId { get; set; }
        /// <summary>
        ///发票校验结果
        /// </summary>
        [MapTo(FieldName = "check_result")]
        public string CheckResult { get; set; }
        /// <summary>
        ///备注
        /// </summary>
        [MapTo(FieldName = "note")]
        public string Note { get; set; }
        /// <summary>
        ///纳税人识别号
        /// </summary>
        [MapTo(FieldName = "identification_number")]
        public string IdentificationNumber { get; set; }
        /// <summary>
        ///报销单位ID
        /// </summary>
        [MapTo(FieldName = "company_id")]
        public int? CompanyId { get; set; }
        /// <summary>
        ///报销单位名称
        /// </summary>
        [MapTo(FieldName = "company_name")]
        public string CompanyName { get; set; }
        /// <summary>
        ///扫描信息
        /// </summary>
        public string ScanInfo { get; set; }
        /// <summary>
        ///导入状态
        /// </summary>
        public string status { get; set; }
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
