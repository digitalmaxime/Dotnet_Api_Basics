using _3_Middleware.EndpointFilters;
using _3_Middleware.HttpClient;
using Microsoft.AspNetCore.Mvc;

namespace _3_Middleware.Endpoints;

public static class BlogEndpoints
{
    public static void AddBlogEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("blog")
            .WithDescription("Blog related endpoints")
            .WithTags("Blog");

        group.MapGet("", ([FromServices] BlogPostHttpClient httpClient) => httpClient.GetBlog());

        group.MapGet("{name}",
                ([FromServices] BlogPostHttpClient httpClient, [FromRoute] string name) => httpClient.GetBlog())
            .AddEndpointFilter<ClassifiedBlogFilter>()
            .WithDescription("Get blog post by id")
            .CacheOutput(policy => policy.Expire(TimeSpan.FromSeconds(60)));

        group.MapGet("exception", _ => throw new ArgumentException("Exception thrown from endpoint"))
            .WithDescription("Get blog post by id");
    }
}