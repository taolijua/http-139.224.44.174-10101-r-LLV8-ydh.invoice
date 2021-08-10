using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities;

namespace YDH.Finace.Entities.Base
{
    /// <summary>
    /// 分页请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageRequestDto<T> : Model
    {

     
        /// <summary>
        /// 第几页
        /// </summary>
        public int Page { get; set; }
        
        /// <summary>
        /// 一页条数
        /// </summary>
        public int limit { get; set; }
       
        /// <summary>
        /// 请求参数对象
        /// </summary>
        public T Dto { get; set; }

    }
}
