using System.Text.Json.Serialization;
using Application.Extensions;
using EFCore_JulieLerman.Extensions;
using EFCore_JulieLerman.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

/*** Services ***/
builder.Services.RegisterPersistenceServices(builder.Configuration);
builder.Services.RegisterApplicationServices();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddAutoMapper(typeof(AuthorMappingProfile).Assembly);

/*** Swagger ***/
builder.Services.AddEndpointsApiExplorer(); // Expose meta data about our application
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsEnvironment("Development"))
{
    using var serviceScope = app.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

/*** Endpoints ***/
app.MapGet("/", context =>
{
    context.Response.Redirect("./swagger/index.html", permanent: false);
    return Task.FromResult(0);
});

app.MapBookReviewEndpoints();
app.MapAuthorEndpoints();

/*** Swagger ***/
app.UseSwagger(); // Generates OpenApi specification
app.UseSwaggerUI(); // Enables the documentation UI

await app.RunAsync();
public partial class Program { }