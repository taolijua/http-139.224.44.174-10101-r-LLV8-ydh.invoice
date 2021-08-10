using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// 按指定数量分割成指定大小的块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="blockSize"></param>
        /// <returns></returns>
        public static List<T[]> Split<T>(this IEnumerable<T> source, int blockSize)
        {
            var unit = new List<T>();
            var result = new List<T[]>();
            var enumerator = source.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                unit.Add(enumerator.Current);
                if (unit.Count == blockSize)
                {
                    result.Add(unit.ToArray());
                    unit = new List<T>();
                }
            }
            if (unit.Count > 0) result.Add(unit.ToArray());
            return result;
        }
        /// <summary>
        /// 检查指定集合是否为空(null也视为空)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpyt<T>(this IEnumerable<T> source)
        {
            return source.IsNull() || source.Count() == 0;
        }
    }
}
