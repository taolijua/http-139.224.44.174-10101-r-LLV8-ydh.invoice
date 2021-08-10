using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YDH.Finace.Common.AllExceptions;
using YDH.Finace.Data;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Binder;
using YDH.Finace.Data.Interfaces;

namespace YDH.Finace.Data
{
    /// <summary>
    /// SqlServer数据库访问上下文
    /// </summary>
    public sealed class SqlServerContext : DBContext, ISqlServerContext
    {
        /// <summary>
        /// SqlServer写入同步锁
        /// </summary>
        private readonly static object _writeLock = new object();

        /// <summary>
        /// 当前事务
        /// </summary>
        private SqlTransaction _transaction;
        /// <summary>
        /// 连接字符串
        /// </summary>
        public readonly string _connectString;
        /// <summary>
        /// 会话选项
        /// </summary>
        private readonly DBContextOptions _options;
        /// <summary>
        /// 实体构建程序
        /// </summary>
        private readonly IEntityBinder _entityBinder;
        /// <summary>
        /// 类型转换提供程序
        /// </summary>
        private readonly IChangeTypeProvider _changeTypeProvider;


        /// <summary>
        /// 是否事务范围
        /// </summary>
        private bool IsTransactionScope => _transaction != null && _transaction.Connection.State == ConnectionState.Open;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="entityBinder"></param>
        public SqlServerContext(IOptions<DBContextOptions> options, IEntityBinder entityBinder, IChangeTypeProvider changeTypeProvider)
        {
            _options = options.Value;
            _entityBinder = entityBinder;
            _connectString = _options.SqlServerConnectString;
            _changeTypeProvider = changeTypeProvider;
            //Trace.WriteLine(this.GetHashCode());
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public SqlTransaction BeginTransaction()
        {
            //if (IsTransactionScope)
            //    throw new FriendlyException("同一时间只能执行一个事务");
            var SqlConnection = GetNewConnection();
            _transaction = SqlConnection.BeginTransaction();
            return _transaction;
        }
        /// <summary>
        /// 获取新连接
        /// </summary>
        /// <returns></returns>
        private SqlConnection GetNewConnection()
        {
            _transaction = null;
            var connection = new SqlConnection(_connectString);
            connection.Open();
            return connection;
        }
        /// <summary>
        /// 获取Text命令
        /// </summary>
        /// <param name="SqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SqlCommand GetTextCommand(SqlConnection SqlConnection,
                                             SqlTransaction transaction,
                                             string commandText,
                                             params SqlParameter[] commandParameters)
        {
            var command = new SqlCommand
            {
                Connection = SqlConnection ?? transaction?.Connection,
                Transaction = null,
                CommandText = commandText,
                CommandType = CommandType.Text
            };
            if (commandParameters != null)
            {
                foreach (SqlParameter value in commandParameters)
                {
                    command.Parameters.Add(value);
                }
            }
            return command;
        }

        #region Entity
        /// <summary>
        /// 执行sql语句，并返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public T Entity<T>(string commandText, params SqlParameter[] commandParameters) where T : IMapping
        {
            var reader = ExecuteReader(commandText, commandParameters);
            return _entityBinder.Bind<T>(reader);
        }
        /// <summary>
        /// 异步执行sql语句，并返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<T> EntityAsync<T>(string commandText, params SqlParameter[] commandParameters) where T : IMapping
        {
            var reader = await ExecuteReaderAsync(commandText, commandParameters);
            return _entityBinder.Bind<T>(reader);
        }
        #endregion

        #region Entity List
        /// <summary>
        /// 执行sql语句，并返回对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public IList<T> List<T>(string commandText, params SqlParameter[] commandParameters) where T : IMapping
        {
            var reader = ExecuteReader(commandText, commandParameters);
            return _entityBinder.BindList<T>(reader);
        }
        /// <summary>
        /// 异步执行sql语句，并返回列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<IList<T>> ListAsync<T>(string commandText, params SqlParameter[] commandParameters) where T : IMapping
        {
            var reader = await ExecuteReaderAsync(commandText, commandParameters);
            return _entityBinder.BindList<T>(reader);
        }
        #endregion

        #region DbDataReader
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="SqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SqlDataReader ExecuteReader(SqlConnection SqlConnection, SqlTransaction transaction, string commandText, params SqlParameter[] commandParameters)
        {
            var SqlCommand = GetTextCommand(SqlConnection, transaction, commandText, commandParameters);
            SqlDataReader result = IsTransactionScope ? SqlCommand.ExecuteReader() : SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
            SqlCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string commandText, params SqlParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteReader(null, _transaction, commandText, commandParameters);
            }
            else
            {
                var SqlConnection = GetNewConnection();
                return ExecuteReader(SqlConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<SqlDataReader> ExecuteReaderAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var taskCompletionSource = new TaskCompletionSource<SqlDataReader>();
            if (CancellationToken == CancellationToken.None || !CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = ExecuteReader(commandText, commandParameters);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            }
            else
            {
                taskCompletionSource.SetCanceled();
            }
            return taskCompletionSource.Task;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="SqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private object ExecuteScalar(SqlConnection SqlConnection, SqlTransaction transaction, string commandText, params SqlParameter[] commandParameters)
        {
            var SqlCommand = GetTextCommand(SqlConnection, transaction, commandText, commandParameters);
            var result = SqlCommand.ExecuteScalar();
            SqlCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params SqlParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteScalar(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var SqlConnection = GetNewConnection();
                return ExecuteScalar(SqlConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 异步执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            if (CancellationToken == CancellationToken.None || !CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = ExecuteScalar(commandText, commandParameters);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            }
            else
            {
                taskCompletionSource.SetCanceled();
            }
            return taskCompletionSource.Task;
        }
        /// <summary>
        /// 执行sql语句，并返回首行首列指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string commandText, params SqlParameter[] commandParameters)
        {
            var result = ExecuteScalar(commandText, commandParameters);
            return _changeTypeProvider.ToPrimitiveType<T>(result);
        }
        /// <summary>
        /// 异步执行sql语句，并返回首行首列指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string commandText, params SqlParameter[] commandParameters)
        {
            var result = await ExecuteScalarAsync(commandText, commandParameters);
            return _changeTypeProvider.ToPrimitiveType<T>(result);
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="SqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(SqlConnection SqlConnection, SqlTransaction transaction, string commandText, params SqlParameter[] commandParameters)
        {
            var result = 0;
            var SqlCommand = GetTextCommand(SqlConnection, transaction, commandText, commandParameters);
            if (Monitor.TryEnter(_writeLock, TimeSpan.FromMilliseconds(999)))
            {
                try
                {
                    result = SqlCommand.ExecuteNonQuery();
                }
                finally
                {
                    Monitor.Exit(_writeLock);
                }
            }
            else
            {
                throw new TimeoutException("等待Sqlite锁的过程超时");
            }
            SqlCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params SqlParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteNonQuery(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var SqlConnection = GetNewConnection();
                return ExecuteNonQuery(SqlConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        ///  执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var taskCompletionSource = new TaskCompletionSource<int>();
            if (CancellationToken == CancellationToken.None || !CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = ExecuteNonQuery(commandText, commandParameters);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            }
            else
            {
                taskCompletionSource.SetCanceled();
            }
            return taskCompletionSource.Task;
        }
        #endregion

        #region ExecuteDataset
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="SqlConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private DataSet ExecuteDataset(SqlConnection SqlConnection, SqlTransaction transaction, string commandText, params SqlParameter[] commandParameters)
        {
            var SqlCommand = GetTextCommand(SqlConnection, transaction, commandText, commandParameters);
            var SqlDataAdapter = new SqlDataAdapter(SqlCommand);
            DataSet dataSet = new DataSet();
            SqlDataAdapter.Fill(dataSet);
            SqlCommand.Parameters.Clear();
            return dataSet;
        }
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string commandText, params SqlParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteDataset(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var SqlConnection = GetNewConnection();
                return ExecuteDataset(SqlConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDatasetAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var taskCompletionSource = new TaskCompletionSource<DataSet>();
            if (CancellationToken == CancellationToken.None || !CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = ExecuteDataset(commandText, commandParameters);
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            }
            else
            {
                taskCompletionSource.SetCanceled();
            }
            return taskCompletionSource.Task;
        }
        #endregion

        #region ExecuteDatatable
        /// <summary>
        /// 执行sql语句，并返回结果表
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, params SqlParameter[] commandParameters)
        {
            var dataSet = ExecuteDataset(commandText, commandParameters);
            if (dataSet == null) return null;
            if (dataSet.Tables.Count == 0) return null;
            return dataSet.Tables[0];
        }
        /// <summary>
        /// 执行sql语句，并返回结果表
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var dataSet = await ExecuteDatasetAsync(commandText, commandParameters);
            if (dataSet == null) return null;
            if (dataSet.Tables.Count == 0) return null;
            return dataSet.Tables[0];
        }
        #endregion

        #region ExecuteDataRow
        /// <summary>
        /// 执行sql语句，并返回结果行
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataRow ExecuteDataRow(string commandText, params SqlParameter[] commandParameters)
        {
            var table = ExecuteDataTable(commandText, commandParameters);
            if (table == null) return null;
            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }
        /// <summary>
        /// 执行sql语句，并返回结果行
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<DataRow> ExecuteDataRowAsync(string commandText, params SqlParameter[] commandParameters)
        {
            var table = await ExecuteDataTableAsync(commandText, commandParameters);
            if (table == null) return null;
            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }
        #endregion
    }
}
