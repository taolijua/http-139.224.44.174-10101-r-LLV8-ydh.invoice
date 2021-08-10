using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.Web.FilterAttributes
{
    /// <summary>
    /// API返回结果处理筛选处理
    /// <para>使用<see cref="SkipResultFilterAttribute" />可以跳过当前筛选器</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ApiResultFilterAttribute : ResultFilterAttribute
    {
        /// <summary>
        /// 结果处理前期可以修改Result
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result.IsNull() || context.Cancel) return;
            // 声明此结果不需要进行处理
            if (context.Filters.Any(filter => filter is ISkipResultFilter)) return;

            if (context.Result is ObjectResult result)
            {
                if (result.Value.IsNull())
                {
                    // ObjectResult但值为空，返回http 204
                    context.Result = new NoContentResult();
                }
                else if (result.DeclaredType.IsBasicValueType())
                {
                    // ObjectResult 值声明是基元值类型，则包装后返回
                    context.Result = new ObjectResult(new { Data = result.Value, Type = result.DeclaredType.Name });
                }
                else if (result.Value.GetType().IsBasicValueType())
                {
                    // ObjectResult 值类型是基元值类型，则包装后返回
                    context.Result = new ObjectResult(new { Data = result.Value, Type = result.Value.GetType().Name });
                }
            }
            else if (context.Result is EmptyResult)
            {
                context.Result = new NoContentResult();
            }
        }

        /// <summary>
        /// 结果处理完成后可以检查是否已取消请求等
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            // 此请求客户端是否已取消
            context.HttpContext.RequestAborted.ThrowIfCancellationRequested();
            // 当前结果已标记为取消
            if (context.Canceled) throw new OperationCanceledException();
            // 抛出结果处理期间异常
            context.ExceptionDispatchInfo?.Throw();
        }

        /// <summary>
        /// 这里为了调试,重写OnResultExecuting与OnResultExecuted后不建议再重写此方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await base.OnResultExecutionAsync(context, next);

            // 以下为 ResultFilterAttribute.OnResultExecutionAsync 实现
            // _ = context ?? throw new ArgumentNullException(nameof(context));
            // _ = next ?? throw new ArgumentNullException(nameof(next));
            // OnResultExecuting(context);
            // if (context.Cancel) return;
            // OnResultExecuted(await next());
        }
    }
}
