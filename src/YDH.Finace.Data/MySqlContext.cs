using Microsoft.Extensions.Options;
using YDH.Finace.Data.Entities;
using YDH.Finace.Data.Entities.Binder;
using YDH.Finace.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Transactions;
using System.Diagnostics;
using YDH.Finace.Data;

namespace YDH.Finace.Data
{
    /// <summary>
    /// MySql数据库访问上下文
    /// </summary>
    public sealed class MySqlContext : DBContext, IMySqlContext
    {
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="entityBinder"></param>
        /// <param name="changeTypeProvider"></param>
        public MySqlContext(IOptions<DBContextOptions> options, IEntityBinder entityBinder, IChangeTypeProvider changeTypeProvider)
        {
            _options = options.Value;
            _entityBinder = entityBinder;
            _connectString = _options.MySqlConnectString;
            _changeTypeProvider = changeTypeProvider;
        }

        #region Entity
        /// <summary>
        /// 执行sql语句，并返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public T Entity<T>(string commandText, params MySqlParameter[] commandParameters) where T : IMapping
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
        public async Task<T> EntityAsync<T>(string commandText, params MySqlParameter[] commandParameters) where T : IMapping
        {
            var reader = await ExecuteReaderAsync(commandText, commandParameters);
            return _entityBinder.Bind<T>(reader);
        }
        #endregion

        #region Entity List
        /// <summary>
        /// 执行sql语句，并返回列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public IList<T> List<T>(string commandText, params MySqlParameter[] commandParameters) where T : IMapping
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
        public async Task<IList<T>> ListAsync<T>(string commandText, params MySqlParameter[] commandParameters) where T : IMapping
        {
            var reader = await ExecuteReaderAsync(commandText, commandParameters);
            return _entityBinder.BindList<T>(reader);
        }
        #endregion

        #region DbDataReader
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public MySqlDataReader ExecuteReader(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySqlHelper.ExecuteReader(_connectString, commandText, commandParameters);
        }
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<MySqlDataReader> ExecuteReaderAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            return await MySqlHelper.ExecuteReaderAsync(_connectString, commandText, CancellationToken, commandParameters);
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySqlHelper.ExecuteScalar(_connectString, commandText, commandParameters);
        }
        /// <summary>
        /// 异步执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            return await MySqlHelper.ExecuteScalarAsync(_connectString, commandText, CancellationToken, commandParameters);
        }
        /// <summary>
        /// 执行sql语句，并返回首行首列指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string commandText, params MySqlParameter[] commandParameters)
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
        public async Task<T> ExecuteScalarAsync<T>(string commandText, params MySqlParameter[] commandParameters)
        {
            var result = await ExecuteScalarAsync(commandText, commandParameters);
            return _changeTypeProvider.ToPrimitiveType<T>(result);
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySqlHelper.ExecuteNonQuery(_connectString, commandText, commandParameters);
        }
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            return await MySqlHelper.ExecuteNonQueryAsync(_connectString, commandText, CancellationToken, commandParameters);
        }
        #endregion

        #region ExecuteDataset
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExecuteDataset(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySqlHelper.ExecuteDataset(_connectString, commandText, commandParameters);
        }
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<DataSet> ExecuteDatasetAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            return await MySqlHelper.ExecuteDatasetAsync(_connectString, commandText, CancellationToken, commandParameters);
        }
        #endregion

        #region ExecuteDataTable
        /// <summary>
        /// 执行sql语句，并返回结果表
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, params MySqlParameter[] commandParameters)
        {
            var dataSet = MySqlHelper.ExecuteDataset(_connectString, commandText, commandParameters);
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
        public async Task<DataTable> ExecuteDataTableAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            var dataSet = await MySqlHelper.ExecuteDatasetAsync(_connectString, commandText, CancellationToken, commandParameters);
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
        public DataRow ExecuteDataRow(string commandText, params MySqlParameter[] commandParameters)
        {
            return MySqlHelper.ExecuteDataRow(_connectString, commandText, commandParameters);
        }
        /// <summary>
        /// 执行sql语句，并返回结果行
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public async Task<DataRow> ExecuteDataRowAsync(string commandText, params MySqlParameter[] commandParameters)
        {
            return await MySqlHelper.ExecuteDataRowAsync(_connectString, commandText, CancellationToken, commandParameters);
        }
        #endregion


        //public void RunTransaction(string myConnString)
        //{
        //    MySqlConnection myConnection = new MySqlConnection(myConnString);
        //    myConnection.Open();
        //    ///
        //    MySqlCommand myCommand = myConnection.CreateCommand();
        //    MySqlTransaction myTrans;
        //    ///
        //    // Start a local transaction
        //    myTrans = myConnection.BeginTransaction();
        //    // Must assign both transaction object and connection
        //    // to Command object for a pending local transaction
        //    myCommand.Connection = myConnection;
        //    myCommand.Transaction = myTrans;
        //    ///
        //    try
        //    {
        //        myCommand.CommandText = "insert into Test (id, desc) VALUES (100, 'Description')";
        //        myCommand.ExecuteNonQuery();
        //        myCommand.CommandText = "insert into Test (id, desc) VALUES (101, 'Description')";
        //        myCommand.ExecuteNonQuery();
        //        myTrans.Commit();
        //        // ("Both records are written to database.");
        //    }
        //    catch (Exception e)
        //    {
        //        try
        //        {
        //            myTrans.Rollback();
        //        }
        //        catch (Exception ex)
        //        {
        //            if (myTrans.Connection != null)
        //            {
        //                // ("An exception of type " + ex.GetType() +
        //                // " was encountered while attempting to roll back the transaction.");
        //            }
        //        }
        //        ///
        //        // ("An exception of type " + e.GetType() +
        //        // " was encountered while inserting the data.");
        //        // ("Neither record was written to database.");
        //    }
        //    finally
        //    {
        //        myConnection.Close();
        //    }
        //}

    }
}
