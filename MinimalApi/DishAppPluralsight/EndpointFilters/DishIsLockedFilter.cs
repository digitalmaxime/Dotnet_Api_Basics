namespace DishAppPluralsight.EndpointFilters;

public class DishIsLockedFilter : IEndpointFilter
{
    private readonly Guid _lockedDishId;

    public DishIsLockedFilter(Guid lockedDishId)
    {
        _lockedDishId = lockedDishId;
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        Guid dishId;
        var methodName = context.HttpContext.Request.Method;
        switch (methodName)
        {
            case "PUT":
                dishId  = context.GetArgument<Guid>(2);
                break;
            case "DELETE":
                dishId = context.GetArgument<Guid>(1);
                break;
            default:
                throw new ArgumentException("failed to get dishId query param");
        }
        
        if (dishId == _lockedDishId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Dish is perfect and cannot be changed..",
                Detail = "You cannot update or delete perfection."
            });
        }
        
        var result = await next.Invoke(context);
        
        return result;
    }
}