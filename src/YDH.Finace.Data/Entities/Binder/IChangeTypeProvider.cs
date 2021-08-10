using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities.Binder
{
    /// <summary>
    /// 类型转换提供程序接口
    /// </summary>
    public interface IChangeTypeProvider
    {
        /// <summary>
        /// 按类型生成转换方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        MethodInfo GetChangeTypeMethodInfo(Type type);
        /// <summary>
        /// 将<see cref="object"/>转换为<see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcValue"></param>
        /// <returns></returns>
        T ToPrimitiveType<T>(object srcValue);
    }
}
