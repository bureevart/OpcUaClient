using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OpcUaClient.Domain.Exceptions;

namespace OpcUaClient.Middleware;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger;

    public CustomExceptionHandlerMiddleware(RequestDelegate next, Serilog.ILogger? logger)
    {
        _logger = logger;
        _next = next;
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;
        switch (exception)
        {
            case EntityNotFoundException entityNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case DbUpdateException dbUpdateException:
                if (dbUpdateException?.InnerException is PostgresException postgresException)
                {
                    switch (postgresException.SqlState)
                    {
                        case PostgresErrorCodes.UniqueViolation:
                            code = HttpStatusCode.UnprocessableEntity;
                            break;
                        default:
                            break;
                    }
                }
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new { error = exception.Message, innerException = exception.InnerException?.Message });
        }
        
        _logger.Error("Message: {Result} Request: {Method} {Path} {Query}", 
            result, context.Request.Method, context.Request.Path, context.Request.QueryString);
        return context.Response.WriteAsync(result);
    }
}