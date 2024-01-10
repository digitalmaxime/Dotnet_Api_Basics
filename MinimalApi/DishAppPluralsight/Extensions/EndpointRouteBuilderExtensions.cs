using DishAppPluralsight.EndpointFilters;
using DishAppPluralsight.EndpointHandlers;
using DishAppPluralsight.Models;

namespace DishAppPluralsight.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var dishesEndpoints = endpointRouteBuilder
            .MapGroup("/dishes")
            .RequireAuthorization();

        var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");

        dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync);
        dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync)
            .WithName("GetDish")
            .WithOpenApi()
            .WithSummary("Get a dish by providing an id.")
            .WithDescription("Dishes are identified by a URI containing a dish identifier. This identifier is a GUID. You can get one specific dish via this endpoint by providing the identifier.");
        dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync).AllowAnonymous()
            .WithOpenApi(operation =>
            {
                operation.Deprecated = true;
                return operation;
            } );
        dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync).AddEndpointFilter<ValidationFilter>()
            .RequireAuthorization("RequireAdminFromBelgium")
            .ProducesValidationProblem(400)
            .Accepts<DishForCreationDto>("application/json");
        dishWithGuidIdEndpoints.MapPut("", DishesHandlers.UpdateDishByIdAsync)
            .AddEndpointFilter(new DishIsLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
            .AddEndpointFilter(new DishIsLockedFilter(new Guid("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06")))
            .RequireAuthorization("RequireAdminFromBelgium");
        dishWithGuidIdEndpoints.MapDelete("", DishesHandlers.DeleteDishByIdAsync)
            .AddEndpointFilter(new DishIsLockedFilter(new Guid("fd630a57-2352-4731-b25c-db9cc7601b16")))
            .AddEndpointFilter(new DishIsLockedFilter(new Guid("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06")))
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }

    public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientsEndpoints = endpointRouteBuilder
            .MapGroup("/dishes/{dishId:guid}/ingredients")
            .RequireAuthorization();

        ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsByDishIdAsync);

        ingredientsEndpoints.MapPost("", void () => throw new NotImplementedException());
    }
}