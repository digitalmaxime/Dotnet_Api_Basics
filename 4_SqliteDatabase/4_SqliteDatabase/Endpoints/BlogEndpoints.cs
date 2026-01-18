using _4_SqliteDatabase.Database;
using _4_SqliteDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace _4_SqliteDatabase.Endpoints;

public static class BlogEndpoints
{
    public static IEndpointRouteBuilder MapBlogEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("blogs");
        group.MapGet("", (BlogContext context) =>
        {
            var blogs = context.Blogs.ToList();
            return blogs;
        });

        group.MapGet("{id:int}", (BlogContext context, int id) =>
        {
            var blog = context.Blogs.FirstOrDefault(b => b.BlogId == id);
            return blog == null ? Results.NotFound() : Results.Ok(blog);
        });

        group.MapPost("", (BlogContext context, [FromBody] Blog blog) =>
        {
            context.Blogs.Add(blog);
            context.SaveChanges();
            return Task.CompletedTask;
        });

        group.MapDelete("{id:int}", (BlogContext context, int id) =>
        {
            context.Blogs.Remove(context.Blogs.First(b => b.BlogId == id));
            context.SaveChanges();
            return Task.CompletedTask;
        });
        
        return endpoints;
    }
}