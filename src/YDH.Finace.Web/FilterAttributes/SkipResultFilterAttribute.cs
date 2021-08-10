using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Web.FilterAttributes
{
    /// <summary>
    /// 跳过结果筛选器接口
    /// </summary>
    public interface ISkipResultFilter : IFilterMetadata
    { }

    /// <summary>
    /// 跳过结果筛选器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class SkipResultFilterAttribute : Attribute, ISkipResultFilter, IFilterMetadata
    {
    }
}
