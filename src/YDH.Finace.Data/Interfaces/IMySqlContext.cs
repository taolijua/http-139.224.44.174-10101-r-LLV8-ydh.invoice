using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Interfaces
{

    /// <summary>
    /// mysql数据库访问上下文接口
    /// </summary>
    public interface IMySqlContext : IDBContext<MySqlParameter, MySqlDataReader>
    {

    }

}
