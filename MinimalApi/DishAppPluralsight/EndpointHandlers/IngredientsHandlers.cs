using AutoMapper;
using DishAppPluralsight.DbContexts;
using DishAppPluralsight.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace DishAppPluralsight.EndpointHandlers;

public static class IngredientsHandlers
{
    public static async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> GetIngredientsByDishIdAsync(DishesDbContext dishesDbContext, IMapper mapper,
        Guid dishId)
    {
        var dish = (await dishesDbContext.Dishes
            .Include(d => d.Ingredients)
            .FirstOrDefaultAsync(d => d.Id == dishId));

        if (dish == null) return TypedResults.NotFound();

        return TypedResults.Ok(mapper.Map<IEnumerable<IngredientDto>>(dish.Ingredients));
    }
}