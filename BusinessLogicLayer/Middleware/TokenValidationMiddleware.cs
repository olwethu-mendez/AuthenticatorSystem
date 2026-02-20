using BusinessLogicLayer.Infrastructure;
using DataAccessLayer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BusinessLogicLayer.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScope)
        {
            _next = next;
            _scopeFactory = serviceScope;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if(endpoint?.Metadata?.GetMetadata<IAuthorizeData>() == null)
            {
                await _next(context);
                return;
            }
            if(endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }
            var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();

            if(string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                if(endpoint?.Metadata?.GetMetadata<IAuthorizeData>() == null)
                {
                    await _next(context);
                    return;
                }
                if(endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                {
                    await _next(context);
                    return;
                }
                var response = "Authorization header missing or invalid.";
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = response
                });
                return;
            }

            var token = authorizationHeader.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var isBlacklisted = await dbContext.TokenBlacklists.AnyAsync(t => t.Token == token);

                    if (isBlacklisted)
                    {
                        var response = "Token has been revoked/logged out.";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = response
                        });
                        return;
                    }
                }
            }
            await _next(context);
        }
    }
}
