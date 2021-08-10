using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using YDH.Finace.Common;
using YDH.Finace.Web.Authentication.YDHAuth;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 数据保护服务扩展
    /// </summary>
    public static class DataProtectionExtensions
    {
        /// <summary>
        /// 添加数据保护服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="hostenv"></param>
        /// <returns></returns>
        public static IDataProtectionBuilder AddDataProtection(this IServiceCollection services, IHostEnvironment hostenv)
        {
            if (string.IsNullOrEmpty(hostenv.ContentRootPath))
                throw new ArgumentException(nameof(hostenv.ContentRootPath));
            // 获取一个X509v3证书
            var certificate = GetCertificate();
            // 当前目录的DataProtectionKeys文件夹，不能有其它类型的xml文件存在
            var directoryInfo = Directory.CreateDirectory(Path.Combine(hostenv.ContentRootPath, "DataProtectionKeys"));
            // 添加数据保护服务，使用统一名称，将密钥保存至指定路径
            var builder = services.AddDataProtection().SetApplicationName("YDH.Port").PersistKeysToFileSystem(directoryInfo);
            // 证书不为空，使用此证书加密密钥，避免出现提示日志
            if(certificate.NotNull())builder.ProtectKeysWithCertificate(certificate);
            return builder;
        }

        /// <summary>
        /// 获取自带的证书
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2 GetCertificate()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resNames = assembly.GetManifestResourceNames().Where(n => n.Contains("YDH.Port")).ToArray();
                var pfxresName = resNames.FirstOrDefault(n => n.EndsWith(".pfx"));
                if (pfxresName.IsEmpty()) throw new ArgumentException(nameof(pfxresName));
                var pfxresStream = assembly.GetManifestResourceStream(pfxresName);
                if (pfxresStream.IsNull()) throw new ArgumentException(nameof(pfxresStream));
                var pfxresBytes = new byte[pfxresStream.Length];
                pfxresStream.Read(pfxresBytes, 0, pfxresBytes.Length);
                return new X509Certificate2(pfxresBytes);
            }
            catch { return null; }
        }
    }
}
namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// 数据保护服务扩展
    /// </summary>
    public static class DataProtectionExtensions
    {
        /// <summary>
        /// 表示混淆值，需要反混淆
        /// </summary>
        private const string DE_PREFIX = "=>";
        /// <summary>
        /// 表示原始值，需要混淆
        /// </summary>
        private const string EN_PREFIX = "=<";
        /// <summary>
        /// 检查是否为输入的待保护配置
        /// </summary>
        private static readonly Regex isInputRegex = new Regex("(\":[\\s\"]?\")" + EN_PREFIX + "(.*)\"", RegexOptions.Multiline);

        /// <summary>
        /// 获取受保护的配置
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationSection GetProtectedSection(this IConfiguration configuration, string key)
        {
            var section = configuration.GetSection(key);
            DeProtected(section);
            return section;
        }
        /// <summary>
        /// 配置文件添加保护
        /// </summary>
        /// <param name="builder"></param>
        public static void EncryptionProtection(this IConfigurationBuilder _)
        {
            /*
             * appsettings.json中的某个Value以EN_PREFIX开头
             * 则将这个值加密并保存至appsettings.json文件
             */
            if (File.Exists("appsettings.json"))
            {
                try
                {
                    var json = File.ReadAllText("appsettings.json");
                    if (isInputRegex.IsMatch(json))
                    {
                        var text = isInputRegex.Replace(json, match => $"{match.Groups[1].Value}{DE_PREFIX}{match.Groups[2].Value.TokenEncryption()}\"");
                        File.WriteAllText("appsettings.json", text);
                    }
                }
                catch { }
            }
        }
        /// <summary>
        /// 解除保护
        /// </summary>
        /// <param name="section"></param>
        private static void DeProtected(IConfigurationSection section)
        {
            if (section.Value.HasValue())
            {
                if (section.Value.StartsWith(DE_PREFIX))
                    section.Value = section.Value.Substring(DE_PREFIX.Length).TokenDecryption();
            }
            else
            {
                foreach (var children in section.GetChildren()) DeProtected(children);
            }
        }
    }
}
