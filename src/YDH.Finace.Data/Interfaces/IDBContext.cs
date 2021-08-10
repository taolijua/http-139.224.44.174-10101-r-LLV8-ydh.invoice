using YDH.Finace.Data.Entities;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System;

namespace YDH.Finace.Data.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDBContext
    {
        /// <summary>
        /// 当前会话的可取消令牌
        /// </summary>
        CancellationToken CancellationToken { set; get; }
    }


    /// <summary>
    /// 数据库访问上下文接口
    /// </summary>
    /// <typeparam name="TDataParameter"></typeparam>
    /// <typeparam name="TDataReader"></typeparam>
    public interface IDBContext<TDataParameter, TDataReader> : IDBContext
        where TDataParameter : DbParameter
        where TDataReader : DbDataReader
    {

        /// <summary>
        /// 执行sql语句，并返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        T Entity<T>(string commandText, params TDataParameter[] commandParameters) where T : IMapping;
        /// <summary>
        /// 异步执行sql语句，并返回一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<T> EntityAsync<T>(string commandText, params TDataParameter[] commandParameters) where T : IMapping;

        /// <summary>
        /// 执行sql语句，并返回列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        IList<T> List<T>(string commandText, params TDataParameter[] commandParameters) where T : IMapping;
        /// <summary>
        /// 异步执行sql语句，并返回列表对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<IList<T>> ListAsync<T>(string commandText, params TDataParameter[] commandParameters) where T : IMapping;


        /// <summary>
        /// 执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        object ExecuteScalar(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 异步执行sql语句，并返回首行首列
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<object> ExecuteScalarAsync(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行sql语句，并返回首行首列指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 异步执行sql语句，并返回首行首列指定类型的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<T> ExecuteScalarAsync<T>(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 异步执行非查询sql语句，并返回影响行数
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQueryAsync(string commandText, params TDataParameter[] commandParameters);

        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        DataSet ExecuteDataset(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行sql语句，并返回结果集
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<DataSet> ExecuteDatasetAsync(string commandText, params TDataParameter[] commandParameters);

        /// <summary>
        /// 执行sql语句，并返回结果表
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        DataTable ExecuteDataTable(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行sql语句，并返回结果表
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<DataTable> ExecuteDataTableAsync(string commandText, params TDataParameter[] commandParameters);

        /// <summary>
        /// 执行sql语句，并返回结果行
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        DataRow ExecuteDataRow(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行sql语句，并返回结果行
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<DataRow> ExecuteDataRowAsync(string commandText, params TDataParameter[] commandParameters);

        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        TDataReader ExecuteReader(string commandText, params TDataParameter[] commandParameters);
        /// <summary>
        /// 执行sql语句，并返回结果流读取器
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        Task<TDataReader> ExecuteReaderAsync(string commandText, params TDataParameter[] commandParameters);
    }
}
