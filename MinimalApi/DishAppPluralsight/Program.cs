using System.Security.Claims;
using AutoMapper;
using DishAppPluralsight.DbContexts;
using DishAppPluralsight.Entities;
using DishAppPluralsight.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Configure Services
builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());
/*** Add DbContext ***/
builder.Services.AddDbContext<DishesDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

var app = builder.Build();
// Configure app (http pipeline and routes)

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/dishes", async Task<Ok<IEnumerable<DishDto>>> (
    DishesDbContext dishesDbContext, ClaimsPrincipal claim, IMapper mapper, [FromQuery] string? name) =>
{
    Console.WriteLine($"\nUser authenticated ? {claim?.Identity?.IsAuthenticated}\n");

    return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
        .Where(d => name == null || d.Name.Contains(name))
        .ToListAsync()));
});

app.MapGet("/dishes/{dishId:guid}", async Task<Results<NotFound, Ok<DishDto>>> (
    [FromServices] DishesDbContext dishesDbContext,
    IMapper mapper,
    Guid dishId) =>
{
    var dish = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dish == null) return TypedResults.NotFound();
    return TypedResults.Ok(mapper.Map<DishDto>(dish));
}).WithName("GetDish");

app.MapGet("/dishes/{dishName:alpha}",
    async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext dishesDbContext, IMapper mapper, string dishName) =>
    {
        var dish = mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
        if (dish == null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<DishDto>(dish));
    });

app.MapGet("/dishes/{dishId:guid}/ingredients",
    async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> (DishesDbContext dishesDbContext, IMapper mapper,
        Guid dishId) =>
    {
        var dish = (await dishesDbContext.Dishes
            .Include(d => d.Ingredients)
            .FirstOrDefaultAsync(d => d.Id == dishId));

        if (dish == null) return TypedResults.NotFound();

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredientDto>>(dish.Ingredients));
    });

app.MapPost("/dishes", async Task<CreatedAtRoute<DishDto>> (
    DishesDbContext dishesDbContext,
    IMapper mapper,
    [FromBody] DishForCreationDto dishForCreationDto) =>
{
    var dishEntity = mapper.Map<DishForCreationDto, Dish>(dishForCreationDto);
    dishesDbContext.Add(dishEntity);
    await dishesDbContext.SaveChangesAsync();

    var dishToReturn = mapper.Map<DishDto>(dishEntity);
    return TypedResults.CreatedAtRoute(dishToReturn, "GetDish", new { dishId = dishToReturn.Id });
});

app.MapPut("/dishes/{dishId:guid}",
    async Task<Results<NotFound, NoContent>> (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId,
        DishForUpdateDto dishForUpdateDto) =>
    {
        var dish = await dishesDbContext.FindAsync<Dish>(dishId);
        if (dish == null) return TypedResults.NotFound();

        // dish.Name = dishForUpdateDto.Name;
        mapper.Map(dishForUpdateDto, dish);

        await dishesDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    });

app.MapDelete("/dishes/{dishId:guid}",
    async Task<Results<NotFound, NoContent>> (DishesDbContext dishesDbContext, Guid dishId) =>
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null) return TypedResults.NotFound();

        dishesDbContext.Remove<Dish>(dishEntity);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    });

// Recreate and migrate database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();