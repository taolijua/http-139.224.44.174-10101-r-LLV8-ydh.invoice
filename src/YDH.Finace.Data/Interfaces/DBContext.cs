using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace YDH.Finace.Data.Interfaces
{
    /// <summary>
    /// 数据库上下文抽象类
    /// </summary>
    public abstract class DBContext : IDBContext
    {
        /// <summary>
        /// 当前会话的可取消令牌
        /// </summary>
        public CancellationToken CancellationToken { set; get; }

    }
}
