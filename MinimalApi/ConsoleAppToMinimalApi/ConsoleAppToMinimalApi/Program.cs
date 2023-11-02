using ConsoleAppToMinimalApi;
using Microsoft.AspNetCore.Mvc;

// ------------------------------------> Builder :)
var builder = WebApplication.CreateBuilder(args);
// <------------------------------------ Builder :)

builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();


// ------------------------------------> Application :)
var app = builder.Build();
// <------------------------------------ Application :)

// Basic
app.MapGet("hello", () => "hello from miniapi :)");

// Return Obj
app.MapGet("helloobj", () => new
{
    Message = "My Obj Message :)"
});

// "Results" in IResultExtensions Microsoft.AspNetCore.Http>Results.Extensions
app.MapGet("user", () => Results.Ok(new User("Max")));

// Accessing the params (Http Context)
app.MapGet("context", (HttpContext context) => { return context.Request.Headers.RequestId; });


// Accessing the params (Http Request)
app.MapGet("special",
    async (HttpRequest req, HttpResponse res) =>
    {
        await res.WriteAsJsonAsync(req.Query);
    });

// POST
app.MapPost("extra/{year:int}", (
    int year, // Path Parameter
    int age,  // Query Parameter (default)
    [FromQuery(Name = "gender")]string sex,
    [FromHeader]string accept, // Header 
    User user, // Body (record)
    IDateTimeProvider dateTimeProvider // DI Service
    ) => new
{
    year, age, sex, accept, user, dateTimeProvider.Now
});

app.Run();

record User(string FullName);