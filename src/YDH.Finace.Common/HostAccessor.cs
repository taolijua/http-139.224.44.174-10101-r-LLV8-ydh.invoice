using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YDH.Finace.Common;
using YDH.Finace.Common.AllExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YDH.Finace.Common
{
    /// <summary>
    /// 主机访问程序接口
    /// </summary>
    public interface IHostAccessor
    {
        /// <summary>
        /// Host对象
        /// </summary>
        IHost Host { get; }
        /// <summary>
        /// 服务提供者
        /// </summary>
        IServiceProvider Services { get; }
        /// <summary>
        /// 程序配置
        /// </summary>
        IConfiguration Configuration { get; }
        /// <summary>
        /// 是否启用
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// 加入服务 因为DI容器在host生成之前建立
        /// </summary>
        /// <param name="host"></param>
        void JoinHost(IHost host);
    }

    /// <summary>
    /// 主机访问程序
    /// </summary>
    public class HostAccessor : IHostAccessor
    {
        /// <summary>
        /// 当前<see cref="HostAccessor" />
        /// </summary>
        public static HostAccessor Current => _current.Value;
        private static readonly Lazy<HostAccessor> _current = new Lazy<HostAccessor>(() => new HostAccessor());

        private IHost _host;
        /// <summary>
        /// Host对象
        /// </summary>
        public IHost Host => _host ?? throw new FriendlyException("HostAccessor没有启用");
        /// <summary>
        /// 服务提供者
        /// </summary>
        public IServiceProvider Services => Host.Services;
        /// <summary>
        /// 程序配置
        /// </summary>
        public IConfiguration Configuration => Services.GetRequiredService<IConfiguration>();
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { private set; get; }

        private HostAccessor() { }

        /// <summary>
        /// 将IHost对象加入HostAccessor
        /// </summary>
        /// <param name="host"></param>
        public void JoinHost(IHost host)
        {
            if (_host.IsNull() && !host.IsNull())
            {
                _host = host;
                Enabled = true;
            }
        }

        /// <summary>
        /// 根据指定的Key获取配置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfiguration(string key)
        {
            return Configuration.GetValue<string>(key);
        }
    }
}

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostAccessorExtensions
    {

        /// <summary>
        /// 构建主机与主机访问程序
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IHost BuildAndHostAccessor(this IHostBuilder builder)
        {
            var host = builder.ConfigureServices(services =>services.AddSingleton<IHostAccessor>(HostAccessor.Current)).Build();
            HostAccessor.Current.JoinHost(host);
            return host;
        }
    }
}


