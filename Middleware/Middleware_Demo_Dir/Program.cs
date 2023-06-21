var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ConsoleLoggerMiddleware>();

var app = builder.Build();

app.Map("/map", MapHandler.Handle);
app.Map("/favicon.ico", () => {});

app.UseMiddleware<ConsoleLoggerMiddleware>();

app.Use(async (ctx, next) =>
{
    System.Console.WriteLine("\n\n\n*** First Use Statement***");
    await next();
});

app.Run(async (context) => // This prevents app.Map from being executed, why?
{
    System.Console.WriteLine("\n\nin App Run()..");
    
    await context.Response.WriteAsync("app Run()...");
});





app.Run();



