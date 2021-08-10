using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.App
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
#if DEBUG
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
#endif            
            var host = Host.CreateDefaultBuilder(args)
                           .ConfigureHostConfiguration(builder => builder.EncryptionProtection())
                           .ConfigureWebHostDefaults(builder =>
                           {
#if DEBUG
                               builder.UseUrls("http://*");
#else
                               builder.UseContentRoot(Assembly.GetExecutingAssembly().GetDirectory());
#endif
                               builder.UseStartup<Startup>();
                               builder.UseWebRoot("AppFiles");
                           }).BuildAndHostAccessor();
            await host.RunAsync();
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            //Trace.WriteLine(e.Exception.Message);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            //Trace.WriteLine(e.Exception.Message);
        }

        /// <summary>
        /// 未处理异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
#if DEBUG
            if (e.IsTerminating)
            {
                Console.WriteLine("程序即将退出...");
                Console.ReadLine();
            }
#endif
        }
    }
}
