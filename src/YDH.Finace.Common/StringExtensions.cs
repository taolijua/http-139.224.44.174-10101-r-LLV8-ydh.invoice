using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtensions
    {
        #region 类型转换
        /// <summary>
        /// 将有后辍（G，M，K）的字符串转整数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32Suffix(this string text, int defaultValue = 0)
        {
            var carry = 1;
            var result = defaultValue;
            var match = Regex.Match(text, "(\\d+)([GMK])$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                text = match.Groups[1].Value;
                var suffix = match.Groups[2].Value.ToUpper();
                switch (suffix)
                {
                    case "G":
                        carry = 1000 * 1000 * 1000;
                        break;
                    case "M":
                        carry = 1000 * 1000;
                        break;
                    case "K":
                        carry = 1000;
                        break;
                }
                result = text.ToInt32(defaultValue) * carry;
            }
            return result;
        }
        /// <summary>
        /// 字符串转整数
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(this string text, int defaultValue = 0)
        {
            var result = defaultValue;
            if (int.TryParse(text, out var value))
            {
                result = value;
            }
            return result;                
        }
        /// <summary>
        /// 字符串转长整型
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToInt64(this string text, long defaultValue = 0)
        {
            var result = defaultValue;
            if (long.TryParse(text, out var value))
            {
                result = value;
            }
            return result;
        }
        /// <summary>
        /// 字符串转十进制数字
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this string text, decimal defaultValue = 0M)
        {
            var result = defaultValue;
            if (decimal.TryParse(text, out var value))
            {
                result = value;
            }
            return result;
        }
        /// <summary>
        /// 字符串转可为空布尔值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBool(this string text, bool defaultValue = false)
        {
            bool result = defaultValue;
            if (bool.TryParse(text, out var value))
            {
                result = value;
            }
            return result;
        }
        /// <summary>
        /// 字符串转时间
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTime(this string text, DateTimeOffset? defaultValue = null)
        {
            var result = defaultValue ?? DateTimeOffset.MinValue;
            if (DateTimeOffset.TryParse(text, out var value))
            {
                result = value;
            }
            return result;
        }
        /// <summary>
        /// 字符串转字节数组
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaultEncoding">UTF-8</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string text, Encoding defaultEncoding = null)
        {
            try
            {
                if (text.IsEmpty()) return Array.Empty<byte>();
                defaultEncoding ??= Encoding.UTF8;
                return defaultEncoding.GetBytes(text);
            }
            catch { return Array.Empty<byte>(); }
        }
        /// <summary>
        /// 字节数组转字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="defaultEncoding"></param>
        /// <returns></returns>
        public static string GetString(this byte[] bytes, Encoding defaultEncoding = null)
        {
            try
            {
                if (bytes == null || bytes.Length == 0) return string.Empty;
                defaultEncoding ??= Encoding.UTF8;
                return defaultEncoding.GetString(bytes);
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 将字符串转换为指定的枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static T? ToEnum<T>(this string text, bool ignoreCase = true) where T : struct, Enum
        {
            if (Enum.TryParse<T>(text, ignoreCase, out var result))
                return result;
            else
                return default;
        }
        #endregion



        #region 空检查
        /// <summary>
        /// 字符串对象是否只存在空引用、空字符、空白字符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string text, bool space = false)
        {
            if (space)
                return string.IsNullOrWhiteSpace(text);
            else
                return string.IsNullOrEmpty(text);
        }
        /// <summary>
        /// 字符串对象存在有效的值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool HasValue(this string text)
        {
            return !text.IsEmpty();
        }
        #endregion

    }
}
