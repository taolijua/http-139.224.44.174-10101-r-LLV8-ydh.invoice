namespace YDH.Finace.Data
{
    /// <summary>
    /// 数据库访问上下文选项
    /// </summary>
    public sealed class DBContextOptions
    {
        /// <summary>
        /// mysql数据库连接字符串
        /// </summary>
        public string MySqlConnectString { set; get; }
        /// <summary>
        /// sqlite数据库连接字符串
        /// </summary>
        public string SqliteConnectString { set; get; }
        /// <summary>
        /// SqlServer数据库连接字符串
        /// </summary>
        public string SqlServerConnectString { get; set; }
        /// <summary>
        /// sqlite写入锁等待超时时间(单位时间获取失败则异常)毫秒
        /// </summary>
        public int SqliteWriteLockWaitTimeout { set; get; }
        /// <summary>
        /// 绑定编译类型
        /// </summary>
        public string BindCompileType { set; get; }
    }
}
