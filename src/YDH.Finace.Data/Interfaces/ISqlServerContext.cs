using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Data.Interfaces;

namespace YDH.Finace.Data.Interfaces
{
    /// <summary>
    /// SqlServer
    /// </summary>
    public interface ISqlServerContext : IDBContext<SqlParameter, SqlDataReader>
    {
    }
}
