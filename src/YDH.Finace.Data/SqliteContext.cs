using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using YDH.Finace.Common.AllExceptions;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Binder;
using YDH.Finace.Data.Interfaces;

namespace YDH.Finace.Data
{
    /// <summary>
    /// Sqlite数据库访问上下文
    /// </summary>
    public sealed class SqliteContext : DBContext, ISqliteContext
    {
        /// <summary>
        /// sqlite写入同步锁
        /// </summary>
        private readonly static object _writeLock = new object();

        /// <summary>
        /// 当前事务
        /// </summary>
        private SQLiteTransaction _transaction;
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
        public SqliteContext(IOptions<DBContextOptions> options, IEntityBinder entityBinder, IChangeTypeProvider changeTypeProvider)
        {
            _options = options.Value;
            _entityBinder = entityBinder;
            _connectString = _options.SqliteConnectString;
            //Trace.WriteLine(this.GetHashCode());
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public SQLiteTransaction BeginTransaction()
        {
            if (IsTransactionScope)
                throw new FriendlyException("同一时间只能执行一个事务");
            var sqliteConnection = GetNewConnection();
            _transaction = sqliteConnection.BeginTransaction();
            return _transaction;
        }
        /// <summary>
        /// 获取新连接
        /// </summary>
        /// <returns></returns>
        private SQLiteConnection GetNewConnection()
        {
            _transaction = null;
            var connection = new SQLiteConnection(_connectString);
            connection.Open();
            return connection;
        }
        /// <summary>
        /// 获取Text命令
        /// </summary>
        /// <param name="sqliteConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SQLiteCommand GetTextCommand(SQLiteConnection sqliteConnection,
                                             SQLiteTransaction transaction,
                                             string commandText,
                                             params SQLiteParameter[] commandParameters)
        {
            var command = new SQLiteCommand
            {
                Connection = sqliteConnection ?? transaction?.Connection,
                Transaction = null,
                CommandText = commandText,
                CommandType = CommandType.Text
            };
            if (commandParameters != null)
            {
                foreach (SQLiteParameter value in commandParameters)
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
        public T Entity<T>(string commandText, params SQLiteParameter[] commandParameters) where T : IMapping
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
        public async Task<T> EntityAsync<T>(string commandText, params SQLiteParameter[] commandParameters) where T : IMapping
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
        public IList<T> List<T>(string commandText, params SQLiteParameter[] commandParameters) where T : IMapping
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
        public async Task<IList<T>> ListAsync<T>(string commandText, params SQLiteParameter[] commandParameters) where T : IMapping
        {
            var reader = await ExecuteReaderAsync(commandText, commandParameters);
            return _entityBinder.BindList<T>(reader);
        }
        #endregion

        #region DbDataReader
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="sqliteConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SQLiteDataReader ExecuteReader(SQLiteConnection sqliteConnection, SQLiteTransaction transaction, string commandText, params SQLiteParameter[] commandParameters)
        {
            var sqliteCommand = GetTextCommand(sqliteConnection, transaction, commandText, commandParameters);
            SQLiteDataReader result = IsTransactionScope ? sqliteCommand.ExecuteReader() : sqliteCommand.ExecuteReader(CommandBehavior.CloseConnection);
            sqliteCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteReader(string commandText, params SQLiteParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteReader(null, _transaction, commandText, commandParameters);
            }
            else
            {
                var sqliteConnection = GetNewConnection();
                return ExecuteReader(sqliteConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<SQLiteDataReader> ExecuteReaderAsync(string commandText, params SQLiteParameter[] commandParameters)
        {
            var taskCompletionSource = new TaskCompletionSource<SQLiteDataReader>();
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
        /// <param name="sqliteConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private object ExecuteScalar(SQLiteConnection sqliteConnection, SQLiteTransaction transaction, string commandText, params SQLiteParameter[] commandParameters)
        {
            var sqliteCommand = GetTextCommand(sqliteConnection, transaction, commandText, commandParameters);
            var result = sqliteCommand.ExecuteScalar();
            sqliteCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params SQLiteParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteScalar(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var sqliteConnection = GetNewConnection();
                return ExecuteScalar(sqliteConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 异步执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(string commandText, params SQLiteParameter[] commandParameters)
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
        public T ExecuteScalar<T>(string commandText, params SQLiteParameter[] commandParameters)
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
        public async Task<T> ExecuteScalarAsync<T>(string commandText, params SQLiteParameter[] commandParameters)
        {
            var result = await ExecuteScalarAsync(commandText, commandParameters);
            return _changeTypeProvider.ToPrimitiveType<T>(result);
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="sqliteConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(SQLiteConnection sqliteConnection, SQLiteTransaction transaction, string commandText, params SQLiteParameter[] commandParameters)
        {
            var result = 0;
            var sqliteCommand = GetTextCommand(sqliteConnection, transaction, commandText, commandParameters);
            if (Monitor.TryEnter(_writeLock, TimeSpan.FromMilliseconds(999)))
            {
                try
                {
                    result = sqliteCommand.ExecuteNonQuery();
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
            sqliteCommand.Parameters.Clear();
            return result;
        }
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params SQLiteParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteNonQuery(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var sqliteConnection = GetNewConnection();
                return ExecuteNonQuery(sqliteConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        ///  执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<int> ExecuteNonQueryAsync(string commandText, params SQLiteParameter[] commandParameters)
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
        /// <param name="sqliteConnection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private DataSet ExecuteDataset(SQLiteConnection sqliteConnection, SQLiteTransaction transaction, string commandText, params SQLiteParameter[] commandParameters)
        {
            var sqliteCommand = GetTextCommand(sqliteConnection, transaction, commandText, commandParameters);
            var sqliteDataAdapter = new SQLiteDataAdapter(sqliteCommand);
            DataSet dataSet = new DataSet();
            sqliteDataAdapter.Fill(dataSet);
            sqliteCommand.Parameters.Clear();
            return dataSet;
        }
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string commandText, params SQLiteParameter[] commandParameters)
        {
            if (IsTransactionScope)
            {
                return ExecuteDataset(null, _transaction, commandText, commandParameters);
            }
            else
            {
                using var sqliteConnection = GetNewConnection();
                return ExecuteDataset(sqliteConnection, null, commandText, commandParameters);
            }
        }
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public Task<DataSet> ExecuteDatasetAsync(string commandText, params SQLiteParameter[] commandParameters)
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
        public DataTable ExecuteDataTable(string commandText, params SQLiteParameter[] commandParameters)
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
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, params SQLiteParameter[] commandParameters)
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
        public DataRow ExecuteDataRow(string commandText, params SQLiteParameter[] commandParameters)
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
        public async Task<DataRow> ExecuteDataRowAsync(string commandText, params SQLiteParameter[] commandParameters)
        {
            var table = await ExecuteDataTableAsync(commandText, commandParameters);
            if (table == null) return null;
            if (table.Rows.Count == 0) return null;
            return table.Rows[0];
        }
        #endregion
    }
}
