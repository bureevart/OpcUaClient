namespace OpcUaClient.Middleware;

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this
        IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
    
    public static IApplicationBuilder UseCustomLoggingHandler(this
        IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomLoggingHandleMiddleware>();
    }
}