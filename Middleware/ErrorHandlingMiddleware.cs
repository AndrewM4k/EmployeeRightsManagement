using System.Net;

namespace EmployeeRightsManagement.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        // ReSharper disable once ConvertToPrimaryConstructor
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
                var requestId = Guid.NewGuid().ToString("N");
                _logger.LogError(ex, "Unhandled exception. RequestId={RequestId}", requestId);
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred.", requestId });
                }
            }
        }
    }
}


