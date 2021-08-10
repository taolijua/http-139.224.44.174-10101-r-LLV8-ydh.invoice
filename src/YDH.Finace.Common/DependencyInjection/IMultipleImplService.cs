using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common.DependencyInjection
{
    /// <summary>
    /// 继承此接口的服务充放同时在容器中注册多个实现
    /// </summary>
    public interface IMultipleImplService : IService
    {
    }
}
