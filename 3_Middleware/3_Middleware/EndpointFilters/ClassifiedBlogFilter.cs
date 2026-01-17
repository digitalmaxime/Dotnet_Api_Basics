using FluentValidation.Results;

namespace _3_Middleware.EndpointFilters;

public class ClassifiedBlogFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {

        var arguments = context.GetArgument<string>(1);
        if (arguments.Contains("classified"))
        {
            var details = new Dictionary<string, string[]>()
            {
                { "content", new[] { "Content cannot contain classified information." } }
            };
            return TypedResults.ValidationProblem(details);
        }
        
        return await next(context);
    }
}