using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common.DependencyInjection;

namespace YDH.Finace.Common.BusinessServices
{
    /// <summary>
    /// 业务服务接口
    /// </summary>
    public interface IBusinessService : IService, ITransient
    {
    }
}
