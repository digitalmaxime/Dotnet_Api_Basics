using _3_Middleware.Endpoints;
using _3_Middleware.HttpClient;
using _3_Middleware.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<BlogPostHttpClient>();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddScoped<LoggingMiddleware>();
builder.Services.AddOutputCache();

var app = builder.Build();

app.UseHttpsRedirection();

app.AddBlogEndpoints();

app.Use(async (ctx, next) =>
{
    Console.WriteLine("*** First app.Use Statement");
    await next();
});

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<LoggingMiddleware>();
app.UseOutputCache();


app.Run();
