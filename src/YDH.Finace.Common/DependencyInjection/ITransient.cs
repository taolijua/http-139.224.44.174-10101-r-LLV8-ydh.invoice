using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YDH.Finace.Common.DependencyInjection
{
    /// <summary>
    /// 此接口标记服务注册为瞬态级别(每个线程、会话是同一实例)
    /// </summary>
    public interface ITransient
    {
    }
}
