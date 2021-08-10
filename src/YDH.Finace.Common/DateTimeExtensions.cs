using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 将时间转换为通用文本格式(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToText(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
