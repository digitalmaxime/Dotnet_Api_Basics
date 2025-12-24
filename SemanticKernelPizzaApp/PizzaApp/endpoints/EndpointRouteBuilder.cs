using Microsoft.AspNetCore.Http.HttpResults;
using PizzaApp.services;
using PizzaModel;

namespace PizzaApp.endpoints;

public static class EndpointRouteBuilder
{
    public static void MapPizzaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/pizza").WithTags("Pizza");

        group.MapGet("/get-pizzas", 
                (IPizzaService pizzaService) => pizzaService.GetAllPizzas())
            .WithName("GetAllPizzas")
            .WithOpenApi();

        group.MapPost("/add-pizza/{name}/{quantity:int:min(1)}",
                (IPizzaService pizzaService, string name, int quantity) => pizzaService.AddPizza(name, quantity))
            .WithName("AddPizza")
            .WithOpenApi();

        group.MapPost("/order-pizza/{name}",
            Results<Ok<Pizza>, Ok<string>, NotFound<string>, BadRequest<string>> (IPizzaService pizzaService, string name) =>
            {
                var desiredPizza = pizzaService.GetPizzaByName(name);
                if (desiredPizza is null)
                {
                    return TypedResults.BadRequest($"No pizza with name {name} exists");
                }

                var orderedPizza = pizzaService.OrderPizza(desiredPizza);
                return orderedPizza is null
                    ? TypedResults.Ok($"{name} pizza is not is stock")
                    : TypedResults.Ok(orderedPizza);
            });
    }
}