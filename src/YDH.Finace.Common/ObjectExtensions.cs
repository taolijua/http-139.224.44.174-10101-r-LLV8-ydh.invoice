using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 对象为null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        /// <summary>
        /// 对象不为null
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool NotNull<T>(this T obj) where T : class
        {
            return !obj.IsNull();
        }



        #region Type
        /// <summary>
        /// 类型是否字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsStringType(this Type type)
        {
            return type.Name.Equals("String");
        }
        /// <summary>
        /// 类型是否基元值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBasicValueType(this Type type)
        {
            return type.GetCode() > TypeCode.DBNull;
        }

        /// <summary>
        /// 值是否基元值类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_"></param>
        /// <returns></returns>
        public static bool IsBasicValueType<T>(this T _) 
        {
            return typeof(T).IsBasicValueType();
        }

        /// <summary>
        /// 返回类型代码
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeCode GetCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }
        #endregion
    }
}
