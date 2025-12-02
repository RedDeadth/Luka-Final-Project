using System.Net;
using System.Text.Json;

namespace FinalProject.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
                KeyNotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
                ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
                InvalidOperationException invEx => (HttpStatusCode.BadRequest, invEx.Message),
                _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
            };

            _logger.LogError(exception, "Error handling request: {Message}", exception.Message);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                status = (int)statusCode,
                message = message,
                details = context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true 
                    ? exception.Message 
                    : null,
                timestamp = DateTime.UtcNow
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
