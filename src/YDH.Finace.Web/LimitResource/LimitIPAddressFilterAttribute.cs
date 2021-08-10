using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Web.FilterAttributes
{
    /// <summary>
    /// 限制IP地址数据
    /// </summary>
    public interface ILimitIPAddressData
    {
        /// <summary>
        /// 是否白名单 默认true
        /// </summary>
        bool IsWhiteList { set; get; }
        /// <summary>
        /// IP地址列表名称
        /// </summary>
        string IPListName { set; get; }
    }
    /// <summary>
    /// 限制IP地址过滤器
    /// </summary>
    public class LimitIPAddressFilterAttribute : Attribute, ILimitIPAddressData, IFilterMetadata
    {
        /// <summary>
        /// 是否白名单 默认true
        /// </summary>
        public bool IsWhiteList { set; get; } = true;
        /// <summary>
        /// IP地址列表名称
        /// </summary>
        public string IPListName { set; get; }
    }
}
