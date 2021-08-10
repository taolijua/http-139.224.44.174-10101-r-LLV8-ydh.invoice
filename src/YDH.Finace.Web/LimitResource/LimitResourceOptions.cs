using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Web.LimitResource
{
    public sealed class LimitResourceOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string[]> IPAddressMap { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public IList<LimitRequestItem> LimitRequestList { set; get; }
    }

    /// <summary>
    /// 限制请求设置项
    /// </summary>
    public sealed class LimitRequestItem
    {
        /// <summary>
        /// 资源，可以为前辍  "*"表示全局匹配
        /// </summary>
        public string Path { set; get; }
        /// <summary>
        /// 是否按客户端限制，False全局限制
        /// </summary>
        public bool Client { set; get; }
        /// <summary>
        /// 并发请求最大数量
        /// </summary>
        public int MaxCount { set; get; }
    }
}
