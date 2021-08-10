using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common.DependencyInjection;

namespace YDH.Finace.Common.Cache
{
    /// <summary>
     /// 缓存接口
     /// </summary>
     public interface ICacheService
    {
        /// <summary>
        ///  新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ExpirtionTime"></param>
        /// <returns></returns>
        bool Add<T>(string key, T value, int ExpirtionTime = 20);


        /// <summary>
        /// 异步获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<T> GetValueAsync<T>(string key, Func<Task<T>> func);
        /// <summary>
        /// 同步获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        T GetValue<T>(string key, Func<T> func);
        /// <summary>
        /// 验证缓存项是否存在
        /// </summary>
        /// <param name="key">缓存Key</param>
        /// <returns></returns>
        bool Exists<T>(string key);

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Remove<T>(string key);
    }
}
