using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// register the DbContext on the container, getting the
// connection string from appSettings   
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
    builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapGet("/dishes",async (DishesDbContext dishesDbContext,
    ClaimsPrincipal claimsPrincipal,
    IMapper mapper,
    string? name) =>
{
    Console.WriteLine($"User authenticated? {claimsPrincipal.Identity?.IsAuthenticated}");

    return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
        .Where(d => name == null || d.Name.Contains(name))
        .ToListAsync()));
});

app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext dishesDbContext, 
    IMapper mapper,
    Guid dishId) => 
{
    var dishEntity = await dishesDbContext.Dishes
        .FirstOrDefaultAsync(d => d.Id == dishId);
    if (dishEntity == null)
    {
        return Results.NotFound();
    }

    return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
});

app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, 
    IMapper mapper,
    string dishName) =>
{
    return mapper.Map<DishDto>(await dishesDbContext.Dishes
        .FirstOrDefaultAsync(d => d.Name == dishName));
});

app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext dishesDbContext,
    IMapper mapper,
    Guid dishId) =>
{
    return mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients);
});



// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}


app.Run();