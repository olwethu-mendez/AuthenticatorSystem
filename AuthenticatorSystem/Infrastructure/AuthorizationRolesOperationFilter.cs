using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.Eventing.Reader;

namespace AuthenticatorSystem.Infrastructure
{
    public class AuthorizationRolesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var controllerAuthorizeAttributes = context.MethodInfo.DeclaringType?
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .ToList();

            var endpointAuthorizeAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .ToList();

            var allowAnonymousAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Concat(context.MethodInfo.DeclaringType!
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>())
                .ToList();

            if(allowAnonymousAttributes.Any() || (!controllerAuthorizeAttributes.Any() && !endpointAuthorizeAttributes.Any()))
            {
                return;
            }

            var effectiveAuthorizeAttributes = endpointAuthorizeAttributes.Any()
                ? endpointAuthorizeAttributes
                : controllerAuthorizeAttributes;

            var authorizedRoles = new List<string>();
            var authorizedPolicies = new List<string>();
            foreach (var authAttr in effectiveAuthorizeAttributes)
            {
                if (!string.IsNullOrEmpty(authAttr.Roles))
                {
                    var roles = authAttr.Roles.Split(',').Select(r => r.Trim());
                    authorizedRoles.AddRange(roles);
                }
                else if(!string.IsNullOrEmpty(authAttr.Policy))
                {
                    var policies = authAttr.Policy.Split(',').Select(p => p.Trim());
                    authorizedPolicies.AddRange(policies);
                }
            }

            string authText = "🔒";
            if(authorizedRoles.Any()) authText += $" - Roles: {string.Join(", ", authorizedRoles.Distinct())} |";
            if(authorizedRoles.Any() && authorizedPolicies.Any()) authText += $" Policies: {string.Join(", ", authorizedPolicies.Distinct())}";
            else if(authorizedPolicies.Any()) authText += $" - Policies: {string.Join(", ", authorizedPolicies.Distinct())}";

            operation.Summary += $" {authText}";// " + (operation.Summary ?? string.Empty);
        }
    }
}
