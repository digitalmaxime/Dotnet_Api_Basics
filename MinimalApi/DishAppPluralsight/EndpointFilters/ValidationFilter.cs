using DishAppPluralsight.Models;
using FluentValidation;
using FluentValidation.Results;

namespace DishAppPluralsight.EndpointFilters;

public class ValidationFilter : IEndpointFilter
{
    private readonly IValidator<DishForCreationDto> _validator;

    public ValidationFilter(IValidator<DishForCreationDto> validator)
    {
        _validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var arg = context.GetArgument<DishForCreationDto>(2);
        ValidationResult validationResult = await _validator.ValidateAsync(arg);
        
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}