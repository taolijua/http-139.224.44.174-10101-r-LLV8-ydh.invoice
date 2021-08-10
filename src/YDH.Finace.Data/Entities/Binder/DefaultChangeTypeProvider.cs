using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YDH.Finace.Data.Entities.Binder
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate T ToType<T>(object value);
    /// <summary>
    /// 默认类型转换提供程序
    /// </summary>
    public sealed class DefaultChangeTypeProvider : IChangeTypeProvider
    {
        #region static member
        /// <summary>
        /// 基础类型转换
        /// </summary>
        private static readonly Dictionary<Type, Delegate> _converts = new Dictionary<Type, Delegate>
        {
            { typeof(bool), new ToType<bool>(Convert.ToBoolean)},
            { typeof(char), new ToType<char>(Convert.ToChar)},
            { typeof(sbyte), new ToType<sbyte>(Convert.ToSByte)},
            { typeof(byte), new ToType<byte>(Convert.ToByte)},
            { typeof(short), new ToType<short>(Convert.ToInt16)},
            { typeof(ushort), new ToType<ushort>(Convert.ToUInt16)},
            { typeof(int), new ToType<int>(Convert.ToInt32)},
            { typeof(uint), new ToType<uint>(Convert.ToUInt32)},
            { typeof(long), new ToType<long>(Convert.ToInt64)},
            { typeof(ulong), new ToType<ulong>(Convert.ToUInt64)},
            { typeof(float), new ToType<float>(Convert.ToSingle)},
            { typeof(double), new ToType<double>(Convert.ToDouble)},
            { typeof(decimal), new ToType<decimal>(Convert.ToDecimal)},
            { typeof(DateTime), new ToType<DateTime>(Convert.ToDateTime)},
            { typeof(DateTimeOffset), new ToType<DateTimeOffset>(ToDateTimeOffset)},
            { typeof(string), new ToType<string>(Convert.ToString)}
        };
        /// <summary>
        /// 转DateTimeOffset
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static DateTimeOffset ToDateTimeOffset(object value)
        {
            var datetime = Convert.ToDateTime(value);
            return new DateTimeOffset(datetime);
        }
        #endregion

        /// <summary>
        /// 排它锁
        /// </summary>
        private readonly object _lock;
        /// <summary>
        /// 基本转换方法
        /// </summary>
        private readonly MethodInfo _changeType;
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly Dictionary<Type, MethodInfo> _cache;

        /// <summary>
        /// 构建实例
        /// </summary>
        public DefaultChangeTypeProvider()
        {
            _lock = new object();
            _changeType = GetType().GetMethod("ChangeType", BindingFlags.Static | BindingFlags.NonPublic);
            // 基本类型
            _cache = new Dictionary<Type, MethodInfo>
            {
                { typeof(object), _changeType.MakeGenericMethod(typeof(object)) },
                { typeof(DBNull), _changeType.MakeGenericMethod(typeof(DBNull)) },
                { typeof(bool), _changeType.MakeGenericMethod(typeof(bool)) },
                { typeof(char), _changeType.MakeGenericMethod(typeof(char)) },
                { typeof(sbyte), _changeType.MakeGenericMethod(typeof(sbyte)) },
                { typeof(byte), _changeType.MakeGenericMethod(typeof(byte)) },
                { typeof(short), _changeType.MakeGenericMethod(typeof(short)) },
                { typeof(ushort), _changeType.MakeGenericMethod(typeof(ushort)) },
                { typeof(int), _changeType.MakeGenericMethod(typeof(int)) },
                { typeof(uint), _changeType.MakeGenericMethod(typeof(uint)) },
                { typeof(long), _changeType.MakeGenericMethod(typeof(long)) },
                { typeof(ulong), _changeType.MakeGenericMethod(typeof(ulong)) },
                { typeof(float), _changeType.MakeGenericMethod(typeof(float)) },
                { typeof(double), _changeType.MakeGenericMethod(typeof(double)) },
                { typeof(decimal), _changeType.MakeGenericMethod(typeof(decimal)) },
                { typeof(DateTime), _changeType.MakeGenericMethod(typeof(DateTime)) },
                { typeof(DateTimeOffset), _changeType.MakeGenericMethod(typeof(DateTimeOffset)) },
                { typeof(string), _changeType.MakeGenericMethod(typeof(string)) }
            };
        }

        /// <summary>
        /// 获取实体项类型转换方法元数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MethodInfo GetChangeTypeMethodInfo(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (_cache.ContainsKey(type))
            {
                return _cache[type];
            }
            return BuildChangeTypeMethod(type);
        }
        /// <summary>
        /// 将<see cref="object"/>转换为<see cref="{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="srcValue"></param>
        /// <returns></returns>
        public T ToPrimitiveType<T>(object srcValue)
        {
            var type = typeof(T);
            if (_converts.ContainsKey(type) &&
                _converts[type] is ToType<T> convert)
                return convert(srcValue);
            return default;
        }
        /// <summary>
        /// 按类型生成转换方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private MethodInfo BuildChangeTypeMethod(Type type)
        {
            // 生成当前类型的转换方法
            var methodInfo = _changeType.MakeGenericMethod(type);
            Add(type, methodInfo);
            return methodInfo;
        }

        /// <summary>
        /// 添加某个类型的转换方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private MethodInfo Add(Type type, MethodInfo methodInfo)
        {
            lock (_lock)
            {
                _cache[type] = methodInfo;
                return methodInfo;
            }
        }
        /// <summary>
        /// 更改类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T ChangeType<T>(object value)
        {
            // 值为null时
            if (value == null) return default;
            // 值转字符串
            var valueString = value.ToString();
            // 字符串为空时
            if (string.IsNullOrEmpty(valueString)) return default;
            // 获取泛型类型
            var type = typeof(T);
            // 值类型相同时
            if (value.GetType() == type) return (T)value;
            // 为基础类型时
            if (_converts.ContainsKey(type) && 
                _converts[type] is ToType<T> convert) return convert(value);

            // 泛型为值类型时
            if (type.IsValueType)
            {
                // 枚举类型
                if (type.IsEnum) return (T)Enum.Parse(type, valueString, true);

                // 可为空类型
                var innerType = Nullable.GetUnderlyingType(type);
                if (innerType != null)
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, innerType);
                    }
                    catch
                    {
                        return default;
                    }
                }

                // 结构体
                return default;
            }
            else
            {
                return default;
            }
        }

    }

}
