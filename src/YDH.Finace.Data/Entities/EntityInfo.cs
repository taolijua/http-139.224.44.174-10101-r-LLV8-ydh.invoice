using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YDH.Finace.Common;
using YDH.Finace.Data.Entities.Attributes;

namespace YDH.Finace.Data.Entities
{
    /// <summary>
    /// 实体对象信息
    /// </summary>
    public sealed class EntityInfo : Dictionary<string, EntityItem>
    {
        /// <summary>
        /// 所属类型
        /// </summary>
        public Type Type { private set; get; }
        /// <summary>
        /// 映射表名
        /// </summary>
        public string TableName { private set; get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public EntityInfo(Type type)
        {
            if (type.IsNull())
                throw new ArgumentNullException(nameof(type));
            Type = type;
            var attribute = Type.GetCustomAttribute<TableAttribute>();
            TableName = string.IsNullOrEmpty(attribute?.TableName) ? string.Empty : attribute.TableName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="method"></param>
        public void Add(string key, PropertyInfo value, MethodInfo method)
        {
            this[key] = new EntityItem(value, method, Type);
        }


    }

    /// <summary>
    /// 实体对象属性信息
    /// </summary>
    public sealed class EntityItem
    {
        /// <summary>
        /// 所属类型
        /// </summary>
        public Type OwnerType { private set; get; }
        /// <summary>
        /// 属性信息
        /// </summary>
        public PropertyInfo PropertyInfo { private set; get; }
        /// <summary>
        /// 转换为属性类型的方法元数据
        /// </summary>
        public MethodInfo ChangeTypeMethodInfo { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="methodInfo"></param>
        /// <param name="ownerType"></param>
        public EntityItem(PropertyInfo propertyInfo, MethodInfo methodInfo, Type ownerType)
        {
            OwnerType = ownerType;
            PropertyInfo = propertyInfo;
            ChangeTypeMethodInfo = methodInfo;
        }
    }
}
