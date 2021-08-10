using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities
{
    /// <summary>
    /// 实体对象信息管理器接口
    /// </summary>
    public interface IEntityInfoManager
    {
        /// <summary>
        /// 获取注册的实体信息
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        EntityInfo GetEntityInfo<TEntity>() where TEntity : IMapping;
        /// <summary>
        /// 注册实体
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        void Resister<TEntity>() where TEntity : IMapping;

    }
}
