using BusinessLogicLayer.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BusinessLogicLayer.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly ILogger _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next/*, ILogger logger*/)
        {
            _next = next;
            //_logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ClientError ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                var response = ex.Message;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = response
                });
                return;
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var response = "An unexpected error occurred.";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = response,
                    more = ex.Message, //will use logger later somehow...
                });
                return;
                //_logger.LogError(ex, "An unexpected error occurred.");
            }
        }
    }
}
