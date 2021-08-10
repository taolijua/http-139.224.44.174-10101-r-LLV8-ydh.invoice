using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YDH.Finace.Common;

namespace YDH.Finace.Web.Swagger
{
    public sealed class AuthenticationHeaderFilter : IOperationFilter
    {
        private static readonly Type _allowAnonymouseType = typeof(AllowAnonymousAttribute);
        private static readonly Type _authorizeType = typeof(AuthorizeAttribute);
        private readonly string _tokenName = null;

        public AuthenticationHeaderFilter(string tokenName)
        {
            _tokenName = tokenName;
        }
        /// <summary>
        /// 附加认证头
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            // 没有定义授权检查特性
            if (!context.MethodInfo.IsDefined(_authorizeType, true))
            {
                // action或controller定义了可匿名访问特性
                if (context.MethodInfo.IsDefined(_allowAnonymouseType, true) ||
                    context.MethodInfo.DeclaringType.IsDefined(_allowAnonymouseType, false))
                {
                    return;
                }
            }
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = _tokenName,
                Description = "认证令牌",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema { Type = "string" },
            });
        }
    }
}
