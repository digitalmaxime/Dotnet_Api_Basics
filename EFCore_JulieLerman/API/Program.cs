using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterPersistenceServices(builder.Configuration);

var app = builder.Build();

if(app.Environment.IsEnvironment("Development"))
{
    using var serviceScope = app.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<LibraryContext>();
    // await context.Database.EnsureCreatedAsync();
    context.Database.Migrate();
}

app.MapGet("/", () => "Hello World!");

app.Run();