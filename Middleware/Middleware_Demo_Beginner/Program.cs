var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ConsoleLoggerMiddleware>();

var app = builder.Build();

app.Map("/map", MapHandler.Handle);
app.Map("/favicon.ico", () => { });

app.UseMiddleware<ConsoleLoggerMiddleware>();

app.Use(async (ctx, next) =>
{
    System.Console.WriteLine("\n\n\n*** First Use Statement***");
    await next();
});

app.MapGet("/hello", () => { Console.WriteLine("Hello"); return "Hello from minimalApi!"; });
app.Map("/somepath", async context =>
{
    System.Console.WriteLine("\n\nin App Map()..");
    await context.Response.WriteAsync("app Map()...");
});

// Ensure app.Run is the last middleware
// app.Run();
app.Run(async (context) =>
{
    System.Console.WriteLine("\n\nin App Run()..");
    await context.Response.WriteAsync("app Run()...");
});

app.Run();