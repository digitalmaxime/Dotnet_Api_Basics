using EFCore_JulieLerman;
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
    context.Database.Migrate();
}

app.MapAuthorEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();