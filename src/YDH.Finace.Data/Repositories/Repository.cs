using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using YDH.Finace.Data.Interfaces;

namespace YDH.Finace.Data.Repositories
{
    /// <summary>
    /// 仓储抽象类
    /// </summary>
    public abstract class Repository : IRepository
    {
        #region IRepository Impl


        /// <summary>
        /// 开启支持异步访问数据库的事务
        /// </summary>
        /// <param name="timeoutSecond"></param>
        /// <param name="scopeOption"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public virtual TransactionScope BeginTransactionAsyncable(
            int? timeoutSecond = null,
            TransactionScopeOption? scopeOption = null,
            IsolationLevel? isolationLevel = null)
        {
            return CreateTransaction(true, timeoutSecond, scopeOption, isolationLevel);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncable"></param>
        /// <param name="timeoutSecond"></param>
        /// <param name="scopeOption"></param>
        /// <param name="isolationLevel"></param>
        /// <exception cref="NullReferenceException">
        /// 当此异常的栈顶为<see cref="MySql.Data.MySqlClient.NativeDriver.ExecutePacket(MySqlPacket packetToExecute)"/>时，
        /// 很大可能是由于此事务作用范围内的MySql语句执行时产生了错语导致事务中止并回滚，事务回滚调用MySql进行回滚操作时，
        /// MySql连接可能因为错误而关闭或已回滚，空引用异常是指MySqlClient与服务器之间连接的抽象流为null
        /// </exception>
        /// <returns></returns>
        private TransactionScope CreateTransaction(
            bool asyncable,
            int? timeoutSecond = null,
            TransactionScopeOption? scopeOption = null,
            IsolationLevel? isolationLevel = null)
        {
            timeoutSecond ??= 600;
            scopeOption ??= TransactionScopeOption.Required;
            isolationLevel ??= IsolationLevel.ReadCommitted;
            var asyncFlowOption = asyncable ? TransactionScopeAsyncFlowOption.Enabled : TransactionScopeAsyncFlowOption.Suppress;

            var transactionOptions = new TransactionOptions()
            {
                Timeout = TimeSpan.FromSeconds(timeoutSecond.Value),
                IsolationLevel = isolationLevel.Value
            };
            return new TransactionScope(scopeOption.Value, transactionOptions, asyncFlowOption);
        }
        #endregion
    }
}
