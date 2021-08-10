using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities.Attributes
{
    /// <summary>
    /// 将属性与数据库字段映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class MapToAttribute : Attribute
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { set; get; }
    }
}
