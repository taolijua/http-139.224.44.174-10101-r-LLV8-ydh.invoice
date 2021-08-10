using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace YDH.Finace.Data.Interfaces
{
    /// <summary>
    /// sqlite数据库访问上下文接口
    /// </summary>
    public interface ISqliteContext : IDBContext<SQLiteParameter, SQLiteDataReader>
    { }
}
