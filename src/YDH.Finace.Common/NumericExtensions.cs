using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 数字相关扩展
    /// </summary>
    public static class NumericExtensions
    {
        /// <summary>
        /// 向下舍入(截断)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal RoundDown(this decimal value, int decimals = 2)
        {
            if (decimals < 0) return value;
            var decimalPlaces = (int)Math.Pow(10, decimals);
            return Math.Truncate(value * decimalPlaces) / decimalPlaces;
        }
        /// <summary>
        /// 向下舍入(截断)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double RoundDown(this double value, int decimals = 2)
        {
            return (double)((decimal)value).RoundDown(decimals);
        }

        /// <summary>
        /// 向上舍入(逢一进十)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal RoundUP(this decimal value, int decimals = 2)
        {
            if (decimals < 0) return value;
            var absVal = Math.Abs(value);
            var symbol = value * 1 / absVal;
            var decimalPlaces = (int)Math.Pow(10, decimals);
            return decimal.Ceiling(absVal * decimalPlaces) / decimalPlaces * symbol;            
        }
        /// <summary>
        /// 向上舍入(逢一进十)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double RoundUp(this double value, int decimals = 2)
        {
            return (double)((decimal)value).RoundUP(decimals);
        }

        /// <summary>
        /// 将数字分割为指定大小的块
        /// </summary>
        /// <param name="value"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static IList<int> Split(this int value, int blockSize)
        {
            var count = value / blockSize;
            var rndNum = value % blockSize;
            var result = Enumerable.Repeat(blockSize, count).ToList();
            result.Add(rndNum);
            return result;
        }
    }
}
