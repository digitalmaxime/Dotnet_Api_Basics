using _2_WebHttpClientDemo.HttpClientServices;
using _2_WebHttpClientDemo.Model;
using Microsoft.AspNetCore.Mvc;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient(); // Used by BasicHttpService

builder.Services.AddHttpClient( // Used by NamedClientService
    "NamedClient",
    client => { client.BaseAddress = new Uri("https://localhost:8080/"); });

builder.Services.AddHttpClient<TypedAndResilientHttpClient>(client => // Automatically registers TypedHttpClientService
    {
        client.BaseAddress = new Uri("https://localhost:8080/");
    })
    .AddStandardResilienceHandler(); // Adds retry policy

builder.Services.AddRefitClient<IGeneratedHttpClientService>() // Refit client
    .ConfigureHttpClient(client => { client.BaseAddress = new Uri("https://localhost:8080/"); });

builder.Services.AddScoped<BasicHttpService>();
builder.Services.AddScoped<NamedHttpClientService>();

var app = builder.Build();

app.MapGet("/basic",
    async ([FromServices] BasicHttpService service) => await service.GetBlogsAsync());
app.MapGet("/named",
    async ([FromServices] NamedHttpClientService service) => await service.GetBlogsAsync());
app.MapGet("/typed",
    async ([FromServices] TypedAndResilientHttpClient client) => await client.GetBlogsAsync());
app.MapGet("/generated",
    async ([FromServices] IGeneratedHttpClientService client) => await client.GetBlogsAsync());
app.MapPost("/post", async ([FromBody] BlogPost blogPost, [FromServices] TypedAndResilientHttpClient client) =>
{
    var response = await client.CreatePostAsyn(blogPost.Title, blogPost.Content);
    return Results.Ok(response);
});

app.Run();