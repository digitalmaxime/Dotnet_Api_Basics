using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ConsoleLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine("Before Request");
        await next(context);
        Console.WriteLine("After Request");
    }
}