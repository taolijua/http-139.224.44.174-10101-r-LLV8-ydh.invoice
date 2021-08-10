using YDH.Finace.Data.Entities.Binder;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YDH.Finace.Data.Entities.Attributes;

namespace YDH.Finace.Data.Entities
{
    /// <summary>
    /// 实体对象信息管理器
    /// </summary>
    public sealed class EntityInfoManager : IEntityInfoManager
    {
        /// <summary>
        /// 类型转换提供者
        /// </summary>
        private readonly IChangeTypeProvider _changeTypeProvider;
        /// <summary>
        /// 实体注册表
        /// </summary>
        
        private readonly ConcurrentDictionary<Type, EntityInfo>  _cache = new ConcurrentDictionary<Type, EntityInfo>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="changeTypeProvider"></param>
        public EntityInfoManager(IChangeTypeProvider changeTypeProvider)
        {
            _changeTypeProvider = changeTypeProvider;
        }

        /// <summary>
        /// 获取注册的实体信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public EntityInfo GetEntityInfo<TEntity>() where TEntity : IMapping
        {
            var type = typeof(TEntity);
            return GetEntityInfo(type);
        }
        /// <summary>
        /// 获取注册的实体信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private EntityInfo GetEntityInfo(Type type)
        {
            if (_cache.ContainsKey(type))
            {
                return _cache[type];
            }
            return Register(type);
        }

        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        public void Resister<TEntity>() where TEntity : IMapping
        {
            var type = typeof(TEntity);
            Register(type);
        }
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private EntityInfo Register(Type type)
        {
            var info = new EntityInfo(type);
            // TODO: 可以检查类特性是否存在继承属性、私有属性的映射
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance );
            foreach (var prop in properties)
            {
                if (prop.CanWrite)
                {
                    var fieldName = prop.Name;
                    var mapperAttr = prop.GetCustomAttribute<MapToAttribute>();
                    var typeMethod = _changeTypeProvider.GetChangeTypeMethodInfo(prop.PropertyType);
                    if (!string.IsNullOrEmpty(mapperAttr?.FieldName)) fieldName = mapperAttr.FieldName;
                    info.Add(fieldName, prop, typeMethod);
                }
            }
            _cache[type] = info;
            return info;
        }

        /// <summary>
        /// 是否实体集合
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsEntityCollect(Type type)
        {
            if (type.IsGenericType)
            {
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    var argType = type.GenericTypeArguments?.First();
                    if (typeof(IMapping).IsAssignableFrom(argType))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }

}
