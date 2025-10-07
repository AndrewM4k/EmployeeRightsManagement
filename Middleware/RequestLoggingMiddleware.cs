namespace EmployeeRightsManagement.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var path = context.Request.Path;
            var method = context.Request.Method;
            await _next(context);
            sw.Stop();
            _logger.LogInformation("{Method} {Path} -> {StatusCode} in {ElapsedMs}ms", method, path, context.Response.StatusCode, sw.ElapsedMilliseconds);
        }
    }
}




