using System.Diagnostics;

namespace FinalProject.API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            _logger.LogInformation(
                "[{RequestId}] Request: {Method} {Path} from {IP}",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var logLevel = context.Response.StatusCode >= 500 ? LogLevel.Error
                    : context.Response.StatusCode >= 400 ? LogLevel.Warning
                    : LogLevel.Information;

                _logger.Log(
                    logLevel,
                    "[{RequestId}] Response: {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
