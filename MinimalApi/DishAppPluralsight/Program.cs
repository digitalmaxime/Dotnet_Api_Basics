using DishAppPluralsight.DbContexts;
using DishAppPluralsight.Extensions;
using DishAppPluralsight.Models;
using DishAppPluralsight.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/****************** Configure Services ******************/
builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IValidator<DishForCreationDto>, DishForCreationDtoValidator>();

builder.Services.AddProblemDetails();

/*** Add DbContext ***/
builder.Services.AddDbContext<DishesDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

/*** Add Authentication + Authorization ***/
builder.Services.AddAuthentication().AddJwtBearer();
    // nuget Microsoft.AspNetCore.Authentication.JwtBearer
    // Options can be set with .AddJwtBearer(opt => ...) but they can also be set through appSettings.json "Authentication": {...}

builder.Services.AddAuthorization();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromBelgium", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("country", "Belgium"));

/*** Add Documentation ***/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*************** Configure app (http pipeline and routes) ***************/

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // app.UseExceptionHandler(configureApplicationBuilder =>
    // {
    //     configureApplicationBuilder.Run(async context =>
    //     {
    //         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //         context.Response.ContentType = "text/html";
    //         await context.Response.WriteAsync("Oupsi daisy.. :(");
    //     });
    // });
}

app.UseHttpsRedirection();
// app.MapGet("/", () => "Hello World!");

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

app.UseAuthentication(); // No strictly necessary, cause builder.Services.Add.. already takes care of it
app.UseAuthorization(); // No strictly necessary, cause builder.Services.Add.. already takes care of it

// Recreate and migrate database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

/*** Swagger ***/
app.UseSwagger();
app.UseSwaggerUI();

app.Run();