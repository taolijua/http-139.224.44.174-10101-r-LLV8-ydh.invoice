using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 系统扩展方法
    /// </summary>
    public static class SysExtensions
    {
        /// <summary>
        /// 获取程序集所处目录
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetDirectory(this Assembly assembly)
        {
            if (assembly.IsNull()) throw new ArgumentNullException(nameof(assembly));
            // 解决单文件生成时的问题
            // 如果location不存在有效值，表示此程序集不是从磁盘文件加载的，默认使用当前程序路径
            var location = assembly.Location.HasValue() ? assembly.Location : AppContext.BaseDirectory;
            return Path.GetDirectoryName(location);
        }

        /// <summary>
        /// 返回以YDH开头的程序集
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static Assembly[] GetYDHAssemblies(this AppDomain appDomain)
        {
            return appDomain.GetAssemblies("YDH.");
        }

        /// <summary>
        /// 获取当前作用域下限定名称前辍的程序集
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="qualifiedPrefixes"></param>
        /// <returns></returns>
        public static Assembly[] GetAssemblies(this AppDomain appDomain, params string[] qualifiedPrefixes)
        {
            if (qualifiedPrefixes.Length == 0) 
                return appDomain.GetAssemblies();
            return appDomain.GetAssemblies().
                             Where(assembly => qualifiedPrefixes.
                             Any(prefix => assembly.FullName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))).
                             ToArray();
        }

        /// <summary>
        /// 为类型列表按继承关系排序，子级在前，父级在后
        /// </summary>
        /// <param name="list"></param>
        public static List<Type> InheritSort(this List<Type> list)
        {
            list.Sort((left, right) =>
            {
                if (left.IsAssignableFrom(right))
                    return 1;
                else if (right.IsAssignableFrom(left))
                    return -1;
                else
                    return 0;
            });
            return list;
        }
        /// <summary>
        /// 获取类型 的限定名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetQualifiedName(this Type type)
        {
            if (type.IsGenericType) 
                return type.AssemblyQualifiedName;
            var separator = new char[] { ',', ' ' };
            var typeName = type.FullName.Split(separator).First();
            var assemblyName = type.Assembly.FullName.Split(separator).First();
            return $"{typeName},{assemblyName}";
        }
    }
}
