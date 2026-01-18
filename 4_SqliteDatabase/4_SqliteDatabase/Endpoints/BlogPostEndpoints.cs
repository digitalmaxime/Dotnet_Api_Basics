using _4_SqliteDatabase.Database;

namespace _4_SqliteDatabase.Endpoints;

public static class BlogPostEndpoints
{
    public static IEndpointRouteBuilder MapBlogPostEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("blogposts");
        
        group.MapGet("", (BlogContext context) =>
        {
            var toto = context.BlogPosts.Count();
            return Task.CompletedTask;
        });

        group.MapGet("{title}", (string title) =>
        {
            return Task.CompletedTask;
        });

        group.MapPost("", () =>
        {
            return Task.CompletedTask;
        });
        
        return endpoints;
    }
}