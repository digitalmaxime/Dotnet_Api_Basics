public class ConsoleLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        System.Console.WriteLine("\n\nConsoleLoggerMiddleware - Before Request");
        await next(context);
        System.Console.WriteLine("\n\nConsoleLoggerMiddleware - After the Request");
    }
}