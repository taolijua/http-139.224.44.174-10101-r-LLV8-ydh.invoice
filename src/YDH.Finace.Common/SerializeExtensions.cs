using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 序列化扩展
    /// </summary>
    public static class SerializeExtensions
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            if (obj.IsNull()) return string.Empty;
            try
            {
                var result = JsonConvert.SerializeObject(obj);
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 反序列化一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string text) 
        {
            if (text.IsEmpty()) return default;
            try
            {
                var result = JsonConvert.DeserializeObject<T>(text);
                return result;
            }
            catch
            {
                return default;
            }
        }
        /// <summary>
        /// 反序列化一个对象
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Deserialize(this string text, Type type)
        {
            if (text.IsEmpty() || type.IsNull()) return null;
            try
            {
                var result = JsonConvert.DeserializeObject(text, type);
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
