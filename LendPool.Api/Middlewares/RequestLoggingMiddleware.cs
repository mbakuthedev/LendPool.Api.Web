using System.Text;

namespace LendPool.Api.Middlewares
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

        public async Task InvokeAsync(HttpContext context)
        {
            if (!RequestLoggingToggle.EnableLogging)
            {
                await _next(context);
                return;
            }

            var ip = GetClientIp(context);
            var method = context.Request.Method;
            var path = context.Request.Path;

            var requestBody = await ReadRequestBody(context.Request);

            var originalResponseBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation(
                    "IP: {IP} | {Method} {Path} | Status: {StatusCode} | RequestBody: {RequestBody}",
                    ip, method, path, context.Response.StatusCode, requestBody);
            }
            finally
            {
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalResponseBodyStream);
                context.Response.Body = originalResponseBodyStream; // Restore the original stream
            }
        }

        private string GetClientIp(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "Unknown";

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            request.Body.Position = 0;

            using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            request.Body.Position = 0;
            return string.IsNullOrWhiteSpace(body) ? "[Empty]" : body;
        }
    }

    public static class RequestLoggingToggle
    {
        public static bool EnableLogging = false;
    }

}
