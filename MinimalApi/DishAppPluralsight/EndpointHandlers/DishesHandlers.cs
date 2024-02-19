using System.Security.Claims;
using AutoMapper;
using DishAppPluralsight.DbContexts;
using DishAppPluralsight.Entities;
using DishAppPluralsight.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishAppPluralsight.EndpointHandlers;

public static class DishesHandlers
{
    public static async Task<Ok<IEnumerable<DishDto>>> GetDishesAsync(
        DishesDbContext dishesDbContext, ClaimsPrincipal claim, IMapper mapper, ILogger<DishDto> logger, [FromQuery] string? name)
    {
        logger.LogInformation("Getting the dishes..");
        Console.WriteLine($"\nUser authenticated ? {claim?.Identity?.IsAuthenticated}\n");

        return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
            .Where(d => name == null || d.Name.Contains(name))
            .ToListAsync()));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByIdAsync(
        [FromServices] DishesDbContext dishesDbContext,
        IMapper mapper,
        Guid dishId)
    {
        var dish = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dish == null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<DishDto>(dish));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByNameAsync(DishesDbContext dishesDbContext,
        IMapper mapper, string dishName)
    {
        var dish = mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName));
        if (dish == null) return TypedResults.NotFound();
        return TypedResults.Ok(mapper.Map<DishDto>(dish));
    }

    public static async Task<CreatedAtRoute<DishDto>> CreateDishAsync(
        DishesDbContext dishesDbContext,
        IMapper mapper,
        [FromBody] DishForCreationDto dishForCreationDto)
    {
        var dishEntity = mapper.Map<DishForCreationDto, Dish>(dishForCreationDto);
        dishesDbContext.Add(dishEntity);
        await dishesDbContext.SaveChangesAsync();

        var dishToReturn = mapper.Map<DishDto>(dishEntity);
        return TypedResults.CreatedAtRoute(dishToReturn, "GetDish", new { dishId = dishToReturn.Id });
    }

    public static async Task<Results<NotFound, NoContent>> UpdateDishByIdAsync(DishesDbContext dishesDbContext,
        IMapper mapper, Guid dishId,
        DishForUpdateDto dishForUpdateDto)
    {
        var dish = await dishesDbContext.FindAsync<Dish>(dishId);
        if (dish == null) return TypedResults.NotFound();

        // dish.Name = dishForUpdateDto.Name;
        mapper.Map(dishForUpdateDto, dish);

        await dishesDbContext.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteDishByIdAsync(DishesDbContext dishesDbContext,
        Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null) return TypedResults.NotFound();

        dishesDbContext.Remove<Dish>(dishEntity);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}