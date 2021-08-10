using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;

namespace YDH.Finace.Entities.Base
{
    /// <summary>
    /// 分页返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseRep<T> : Entity
    {
        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> Data { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int Code { get; set; } = 0;
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
    }
}
