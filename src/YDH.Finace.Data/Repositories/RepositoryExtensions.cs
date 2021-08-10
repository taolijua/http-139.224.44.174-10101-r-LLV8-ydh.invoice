using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Port.Data.Repositories
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static int ToInt32(this object @object)
        {
            if (int.TryParse(@object?.ToString(), out var result))
            {
                return result;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static long ToInt64(this object @object)
        {
            if (long.TryParse(@object.ToString(), out var result))
            {
                return result;
            }
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object @object)
        {
            if (DateTime.TryParse(@object?.ToString(), out var result))
            {
                return result;
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public static bool ToBool(this object @object)
        {
            if (bool.TryParse(@object?.ToString(), out var result))
            {
                return result;
            }
            return false;
        }
    }
}
