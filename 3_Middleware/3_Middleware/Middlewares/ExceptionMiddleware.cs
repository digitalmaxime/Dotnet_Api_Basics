using System.Net;
using System.Text.Json;

namespace _3_Middleware.Middlewares;

public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        
        var (statusCode, message) = GetStatusCodeAndMessage(exception);
        
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Type = exception.GetType().Name,
            TraceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(errorResponse, JsonOptions);
        
        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception exception)
    {
        return exception switch
        {
            BadHttpRequestException badRequest => 
                (HttpStatusCode.BadRequest, badRequest.Message),
            
            KeyNotFoundException => 
                (HttpStatusCode.NotFound, "The requested resource was not found"),
            
            UnauthorizedAccessException => 
                (HttpStatusCode.Forbidden, "You do not have permission to access this resource"),
            
            ArgumentException argException => 
                (HttpStatusCode.BadRequest, argException.Message),
            
            InvalidOperationException => 
                (HttpStatusCode.BadRequest, exception.Message),
            
            _ => 
                (HttpStatusCode.InternalServerError, exception.Message)
        };
    }

    private sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public string TraceId { get; init; } = string.Empty;
    }
}