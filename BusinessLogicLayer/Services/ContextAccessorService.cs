using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using BusinessLogicLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class ContextAccessorService
    {
        private readonly IUrlHelper? _urlHelper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ContextAccessorService(IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            if (_contextAccessor?.HttpContext == null)
            {
                throw new ArgumentNullException(nameof(_contextAccessor.HttpContext), "HttpContext cannot be null");
            }

            var actionContext = new ActionContext
            {
                HttpContext = _contextAccessor.HttpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            _urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
        }
        public string? GenerateUrl(string action, string controller, IDictionary<string, object>? routeValues, string? protocol = "https")
        {
            return _urlHelper?.Action(action, controller, routeValues, protocol);
        }

        public string? GetCurrentUserId()
        {
            return _urlHelper?.ActionContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string? GetRequestHeaders(string header)
        {
            return _urlHelper?.ActionContext.HttpContext.Request.Headers[header];
        }
        public string? GetResponseHeaders(string header)
        {
            return _urlHelper?.ActionContext.HttpContext.Response.Headers[header];
        }

        public List<string>? GetUserRoles()
        {
            return _urlHelper?.ActionContext.HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        }

        public bool? CurrentUserIsInRole(string roleName)
        {
            return _urlHelper?.ActionContext.HttpContext.User.IsInRole(roleName);
        }

        public bool? UserIsSignedIn()
        {
            return _urlHelper?.ActionContext.HttpContext.User.Identity?.IsAuthenticated ?? false;
        }

    }
}
