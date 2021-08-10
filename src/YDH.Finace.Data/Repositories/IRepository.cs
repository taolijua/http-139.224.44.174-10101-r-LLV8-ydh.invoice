using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YDH.Finace.Common.DependencyInjection;

namespace YDH.Finace.Data.Repositories
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IRepository : IService, IScoped
    {
        /// <summary>
        /// 开启支持异步访问数据库的事务
        /// </summary>
        /// <param name="timeoutSecond"></param>
        /// <param name="scopeOption"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        TransactionScope BeginTransactionAsyncable(int? timeoutSecond = null, TransactionScopeOption? scopeOption = null, IsolationLevel? isolationLevel = null);
       
    }
}
