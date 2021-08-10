using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;

namespace YDH.Finace.Entities.Base
{
    /// <summary>
    /// 全局返回
    /// </summary>
    public class ReturnDataDto<T> : Model
    {
        /// <summary>
        ///返回码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public IList<T> Data { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Dto { get; set; }

    }
}
