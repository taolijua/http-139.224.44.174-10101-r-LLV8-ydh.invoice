using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace YDH.Finace.Common.Cache
{
    /// <summary>
    /// 缓存接口实现
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        protected IMemoryCache _cache;
        private object obj = new object();

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool Add<T>(string key, T value, int ExpirtionTime = 20)
        {
            Type t = typeof(T);
            if (!t.IsPrimitive) key = key + t.Name;
            lock (obj)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    _cache.Set(key, value, DateTimeOffset.Now.AddMinutes(ExpirtionTime));
                }
            }
            return true;
        }

        public bool Remove<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            Type t = typeof(T);
            if (!t.IsPrimitive) key = key + t.Name;
            if (Exists<T>(key))
            {
                _cache.Remove(key);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 异步的获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<T> GetValueAsync<T>(string key, Func<Task<T>> func)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }
            if (!Exists<T>(key))
            {
                 Add<T>(key, await func());
            }
            if (Exists<T>(key))
            {
                Type t = typeof(T);
                if (!t.IsPrimitive) key = key + t.Name;
                return TransExpV2<T, T>.Trans((T)_cache.Get(key));
            }
            return default(T);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public T GetValue<T>(string key, Func<T> func)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default(T);
            }

            if (!Exists<T>(key))
            {
                Add<T>(key,func());
            }
            if (Exists<T>(key))
            {
                Type t = typeof(T);
                if (!t.IsPrimitive) key = key + t.Name;
                return TransExpV2<T, T>.Trans((T)_cache.Get(key));
            }
            return default(T);
        }

        public bool Exists<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            Type t = typeof(T);
            if (!t.IsPrimitive) key = key + t.Name;
            object cache;
            return _cache.TryGetValue(key, out cache);
        }

        /// <summary>
        /// 主从复制
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        public static class TransExpV2<TIn, TOut >
        {

            private static readonly Func<TIn, TOut> cache = GetFunc();
            private static Func<TIn, TOut> GetFunc()
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
                List<MemberBinding> memberBindingList = new List<MemberBinding>();

                foreach (var item in typeof(TOut).GetProperties())
                {
                    if (!item.CanWrite)
                        continue;

                    MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, property);
                    memberBindingList.Add(memberBinding);
                }

                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

                return lambda.Compile();
            }

            public static TOut Trans(TIn tIn)
            {
                return cache(tIn);
            }

        }
    }
}
