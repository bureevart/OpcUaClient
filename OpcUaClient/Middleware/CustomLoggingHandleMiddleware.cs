
namespace OpcUaClient.Middleware;

public class CustomLoggingHandleMiddleware
{
    private readonly Serilog.ILogger _logger;
    private readonly RequestDelegate _next;


    public CustomLoggingHandleMiddleware(RequestDelegate next, Serilog.ILogger? logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.Information("Request: {Method} {Path} {Query}", context.Request.Method, context.Request.Path, context.Request.QueryString);

        await _next(context);
    }
}

