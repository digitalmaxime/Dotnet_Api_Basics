namespace _3_Middleware.Middlewares;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        logger.LogInformation("*** Middleware invocation before ⬇️");
        await next(context);
        logger.LogInformation("*** Middleware invocation after ⬆️");
    }
}