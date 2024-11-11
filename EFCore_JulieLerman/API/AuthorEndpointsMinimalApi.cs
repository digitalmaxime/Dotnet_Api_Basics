using System.Security.Cryptography.X509Certificates;
using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace EFCore_JulieLerman;

public static class AuthorEndpointsMinimalApi
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("api/author").WithTags(nameof(Author));

        group.MapGet("", async (LibraryContext context) => await context.Authors
                .AsNoTracking()
                .Take(1000)
                .ToListAsync())
            .Produces<List<Author>>(StatusCodes.Status200OK)
            .WithName("GetAllAuthors")
            .WithOpenApi();

        group.MapGet("/{id}", new Func<LibraryContext, int, Task<Results<NotFound, Ok<Author>>>>(
                async (LibraryContext context, int id) =>
                {
                    var author = await context.Authors
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == id);

                    return author is not null
                        ? TypedResults.Ok(author)
                        : TypedResults.NotFound();
                }))
            .Produces<Author>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetAuthorById")
            .WithOpenApi();

        group.MapPut("/{id}",
                async Task<Results<Ok, NotFound, BadRequest>> (int authorId, Author author, LibraryContext context) =>
                {
                    if (authorId != author.Id)
                    {
                        return TypedResults.BadRequest();
                    }

                    context.Entry(author).State = EntityState.Modified;

                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Ok();
                })
            .Produces<Ok>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateAuthor")
            .WithOpenApi();

        group.MapPost("",
            async Task<Results<Created, BadRequest>> (Author author, LibraryContext context) =>
            {
                context.Authors.Add(author);
                await context.SaveChangesAsync();

                return TypedResults.Created($"/api/Author/{author.Id}");
            })
            .Produces<Created>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateAuthor")
            .WithOpenApi();

        group.MapDelete("/{id}",
                async Task<Results<NoContent, NotFound>> (int id, LibraryContext context) =>
                {
                    var author = await context.Authors.FindAsync(id);

                    if (author is null)
                    {
                        return TypedResults.NotFound();
                    }

                    context.Authors.Remove(author);
                    await context.SaveChangesAsync();

                    return TypedResults.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteAuthor")
            .WithOpenApi();
    }
}