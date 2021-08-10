using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace YDH.Finace.Common.Swagger
{


    /// <summary>
    /// 
    /// </summary>
    public class HiddenApiFilter : IDocumentFilter, ISchemaFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
                if (apiDescription.CustomAttributes().Any(i => i is HiddenApiDocumentAttribute))
                {
                    var key = $"/{apiDescription.RelativePath?.Split('?').First()}";
                    swaggerDoc.Paths.Remove(key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties.Count == 0) return;

            if (context.Type.GetCustomAttribute<HiddenApiDocumentAttribute>().NotNull())
            {
                schema.Properties.Clear();
            }
        }

    }

    /// <summary>
    /// 隐藏标注的API文档
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class HiddenApiDocumentAttribute : Attribute
    { }
}
