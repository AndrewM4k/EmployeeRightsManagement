namespace EmployeeRightsManagement.Middleware
{
    public class ResponseCachingHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseCachingHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (HttpMethods.IsGet(context.Request.Method))
            {
                // Be conservative: only cache navigational HTML responses.
                // Do NOT cache JSON/API responses like /User/GetMyRights.
                var accept = context.Request.Headers["Accept"].ToString();
                var isHtmlRequest = accept.Contains("text/html", StringComparison.OrdinalIgnoreCase);

                if (isHtmlRequest)
                {
                    context.Response.OnStarting(state =>
                    {
                        var httpContext = (HttpContext)state!;
                        if (httpContext.Response.StatusCode == 200 && !httpContext.Response.HasStarted)
                        {
                            httpContext.Response.Headers["Cache-Control"] = "public, max-age=60";
                        }
                        return Task.CompletedTask;
                    }, context);
                }
            }

            await _next(context);
        }
    }
}


