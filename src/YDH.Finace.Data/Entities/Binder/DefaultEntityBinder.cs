using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities.Binder
{
    /// <summary>
    /// 默认实体绑定程序
    /// </summary>
    public sealed class DefaultEntityBinder : IEntityBinder
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly bool _isEmit;
        /// <summary>
        /// 会话选项
        /// </summary>
        private readonly DBContextOptions _options;
        /// <summary>
        /// 名称索引器方法元数据
        /// </summary>
        private readonly MethodInfo _getIndexValue;
        /// <summary>
        /// 实体对象信息管理
        /// </summary>
        private readonly IEntityInfoManager _entityInfoManager;
        /// <summary>
        /// 对象转换委托
        /// </summary>
        private readonly ConcurrentDictionary<int, Delegate> _cache;


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="entityInfoManager"></param>
        /// <param name="options"></param>
        public DefaultEntityBinder(IEntityInfoManager entityInfoManager, IOptions<DBContextOptions> options)
        {
            _options = options.Value;
            _entityInfoManager = entityInfoManager;
            _cache = new ConcurrentDictionary<int, Delegate>();
            _getIndexValue = typeof(DbDataReader).GetMethod("GetValue", new Type[] { typeof(int) });
            _isEmit = _options.BindCompileType?.Equals("emit", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        /// <summary>
        /// 将<see cref="DbDataReader"/>转换为<see cref="IMapping"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public T Bind<T>(DbDataReader reader) where T : IMapping
        {
            try
            {
                var bindMethod = GetBindMethod<T>(reader);
                if (reader.Read() && bindMethod != null)
                {
                    return bindMethod.Invoke(reader);
                }
                return default;
            }
            finally
            {
                reader.Close();
            }
        }
        /// <summary>
        /// 将<see cref="DbDataReader"/>转换为<see cref="IList{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public IList<T> BindList<T>(DbDataReader reader) where T : IMapping
        {
            try
            {
                var result = new List<T>();
                var bindMethod = GetBindMethod<T>(reader);
                if (bindMethod != null)
                {
                    while (reader.Read())
                    {
                        var item = bindMethod.Invoke(reader);
                        result.Add(item);
                    }
                    reader.Close();
                }
                return result;
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 获取一个类型的绑定方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private Func<DbDataReader, T> GetBindMethod<T>(DbDataReader reader) where T : IMapping
        {
            var type = typeof(T);
            var fields = GetFieldNames(reader);
            var cacheKey = GenerateKey(fields, type);
            if (_cache.ContainsKey(cacheKey))
            {
                return _cache[cacheKey] as Func<DbDataReader, T>;
            }
            var entityInfo = _entityInfoManager.GetEntityInfo<T>();
            var mapperInfo = GenerateMapperInfo(entityInfo, fields);
            return _isEmit ? EmitBindMethod<T>(mapperInfo) : ExpressionBindMethod<T>(mapperInfo);
        }


        /// <summary>
        /// 生成缓存键
        /// </summary>
        /// <param name="list"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private int GenerateKey(List<string> list, Type type)
        {
            return $"{type.Name}:{string.Join(',', list)}".GetHashCode();
        }

        /// <summary>
        /// 生成映射信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private Dictionary<int, EntityItem> GenerateMapperInfo(EntityInfo info, List<string> fields)
        {
            var mapper = new Dictionary<int, EntityItem>();
            foreach (var prop in info)
            {
                // 这里也匹配属性自身的名称，但以MapTo优先
                var index = fields.FindIndex(i => i.Equals(prop.Key, StringComparison.OrdinalIgnoreCase) ||
                                                  i.Equals(prop.Value.PropertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                if (index >= 0) mapper[index] = prop.Value;
            }
            return mapper;
        }
        /// <summary>
        /// 表达式树生成动态方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private Func<DbDataReader, T> ExpressionBindMethod<T>(Dictionary<int, EntityItem> mapper) where T : IMapping
        {
            var parameter = Expression.Parameter(typeof(DbDataReader), "reader");
            var memberBindingList = new List<MemberBinding>();
            foreach (var info in mapper)
            {
                var oriValue = Expression.Call(parameter, _getIndexValue, Expression.Constant(info.Key));
                //var newValue = Expression.Call(info.Value.ChangeTypeMethodInfo, oriValue, Expression.Constant(info.Value.TypeCode));
                var newValue = Expression.Call(info.Value.ChangeTypeMethodInfo, oriValue);
                var propBind = Expression.Bind(info.Value.PropertyInfo, newValue);
                memberBindingList.Add(propBind);
            }
            var expression = Expression.MemberInit(Expression.New(typeof(T)), memberBindingList);
            var lambda = Expression.Lambda<Func<DbDataReader, T>>(expression, parameter);
            var func = lambda.Compile();
            return func;
        }
        /// <summary>
        /// Emit生成动态方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private Func<DbDataReader, T> EmitBindMethod<T>(Dictionary<int, EntityItem> mapper) where T : IMapping
        {
            var type = typeof(T);
            //var method = new DynamicMethod($"ConvertTo{type.Name}",
            //                 MethodAttributes.Public | MethodAttributes.Static,
            //                 CallingConventions.Standard, type, new Type[] { typeof(DbDataReader) }, type.Module, true);
            var method = new DynamicMethod($"ConvertTo{type.Name}", type, new Type[] { typeof(DbDataReader) }, true);
            var il = method.GetILGenerator();
            // 声明一个 T 类型本地变量
            var result = il.DeclareLocal(type);
            // 新建T的实例
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            // 将实例赋给result，并从栈上弹出
            il.Emit(OpCodes.Stloc, result);
            foreach (var info in mapper)
            {
                #region result.prop = value
                // 第一个本地变量压栈
                il.Emit(OpCodes.Ldloc_0);

                #region object = DbDataReader.GetValue(int)
                // 第一个参数压栈 
                il.Emit(OpCodes.Ldarg_0);
                // 将索引压栈
                il.Emit(OpCodes.Ldc_I4, info.Key);
                // 将索引器读到的值压栈
                il.Emit(OpCodes.Callvirt, _getIndexValue);
                #endregion

                #region value = ConvertToType(object, TypeCode)
                // 将当前属性的类型代码压栈
                //il.Emit(OpCodes.Ldc_I4, (int)info.Value.TypeCode);
                // 转换栈顶的值为当前属性的类型
                il.Emit(OpCodes.Call, info.Value.ChangeTypeMethodInfo);
                #endregion

                // 赋值给当前属性
                il.Emit(OpCodes.Callvirt, info.Value.PropertyInfo.GetSetMethod());
                #endregion
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            // 创建一个匿名委托
            var convert = method.CreateDelegate(typeof(Func<DbDataReader, T>));
            return convert as Func<DbDataReader, T>;
        }

        /// <summary>
        /// 生成字段列表
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        private static List<string> GetFieldNames(IDataRecord record)
        {
            var list = new List<string>();
            for (var i = 0; i < record.FieldCount; i++)
            {
                list.Add(record.GetName(i));
            }
            return list;
        }

        /// <summary>
        /// 生成字段列表
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private static List<string> GetFieldNames(DataTable table)
        {
            var array = new string[table.Columns.Count];
            foreach (DataColumn dc in table.Columns)
            {
                array[dc.Ordinal] = dc.ColumnName;
            }
            return array.ToList();
        }
    }
}
