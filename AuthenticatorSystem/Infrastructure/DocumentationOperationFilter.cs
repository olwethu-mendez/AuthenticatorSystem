using BusinessLogicLayer.Infrastructure.Documentation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticatorSystem.Infrastructure
{
    public class DocumentationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var docAttribute = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<EndpointDocumentationAttribute>()
                .FirstOrDefault();

            if (docAttribute != null && DocStore.AllEndpoints.TryGetValue(docAttribute.Key, out var content))
            {
                operation.Summary = content.Summary;
                operation.Description += content.Remarks;

                // Clear existing responses if you want full control
                operation.Responses.Clear();

                foreach (var resp in content.Responses)
                {
                    operation.Responses.Add(resp.Code.ToString(), new OpenApiResponse
                    {
                        Description = resp.Description
                    });
                }
            }
        }
    }
}
