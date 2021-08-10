using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities.Binder
{
    /// <summary>
    /// 实体绑定程序接口
    /// </summary>
    public interface IEntityBinder
    {
        /// <summary>
        /// 将<see cref="DbDataReader"/>绑定到<see cref="IMapping"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        TEntity Bind<TEntity>(DbDataReader reader) where TEntity : IMapping;
        /// <summary>
        /// 将<see cref="DbDataReader"/>转换为<see cref="IList{T}"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        IList<TEntity> BindList<TEntity>(DbDataReader reader) where TEntity : IMapping;
    }
}
