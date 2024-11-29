using System.Collections.ObjectModel;
using AutoMapper;
using Domain;
using EFCore_JulieLerman.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace EFCore_JulieLerman.Extensions;

public static class AuthorEndpointsExtension
{
    public static void MapAuthorEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("api/author").WithTags(nameof(Author));

        group.MapGet("", async Task<Results<Ok<List<AuthorDto>>, UnauthorizedHttpResult>> (LibraryContext context) =>
            {
                var authorsDtos = await context.Authors
                    .AsNoTracking()
                    .Include(a => a.Books)
                    .Take(1000)
                    .Select(x => new AuthorDto()
                    {
                        Name = x.Name,
                        Books = x.Books.Select(b => new BookDto { Title = b.Title }).ToList()
                    })
                    .ToListAsync();

                return TypedResults.Ok(authorsDtos);
            })
            .Produces<List<AuthorDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithName("GetAllAuthors")
            .WithOpenApi();

        group.MapGet("/{authorId}", async Task<Results<NotFound, Ok<AuthorDto>>>
                (LibraryContext context, IMapper mapper, int authorId) =>
            {
                var author = await context.Authors
                    .AsNoTracking()
                    .Include(a => a.Books)
                    .FirstOrDefaultAsync(a => a.Id == authorId);

                if (author is null) return TypedResults.NotFound();

                return TypedResults.Ok(mapper.Map<AuthorDto>(author));
            })
            .Produces<Author>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetAuthorById")
            .WithOpenApi();

        group.MapPut("/{authorId}",
                async Task<Results<Ok<AuthorDto>, NotFound, BadRequest>> (int authorId, AuthorDto authorDto,
                    LibraryContext context) =>
                {
                    /*
                    var author = await context.Authors.FindAsync(authorId);

                    if (author is null) return TypedResults.NotFound();

                    context.Entry(author).State = EntityState.Modified;
                    author.Name = authorDto.Name;
                    context.ChangeTracker.DetectChanges();
                    var debugView = context.ChangeTracker.DebugView.ShortView;
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.Ok();
                    */
                    
                    var affected = await context.Authors
                        .Where(a => a.Id == authorId)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(a => a.Name, authorDto.Name));

                    return affected == 1 ? TypedResults.Ok(authorDto) : TypedResults.NotFound();
                })
            .Produces<Ok>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateAuthorName")
            .WithOpenApi();

        group.MapPost("",
                async Task<Results<Created<Author>, BadRequest, Conflict>> (AuthorDto authorDto,
                    LibraryContext context) =>
                {
                    var alreadyExists = await context.Authors
                        .AnyAsync(a => a.Name == authorDto.Name);

                    if (alreadyExists)
                    {
                        return TypedResults.Conflict();
                    }

                    var newAuthor = new Author
                    {
                        Name = authorDto.Name,
                        Books = authorDto.Books.Select(x => new Book { Title = x.Title }).ToList()
                    };

                    var createdAuthor = context.Authors.Add(newAuthor);

                    await context.SaveChangesAsync();

                    return TypedResults.Created($"/api/Author/{createdAuthor.Entity.Id}", createdAuthor.Entity);
                })
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .WithName("CreateAuthor")
            .WithOpenApi();

        group.MapDelete("/{authorId}",
                async Task<Results<NoContent, NotFound>> (int authorId, LibraryContext context) =>
                {
                    /*
                    var author = await context.Authors.FindAsync(authorId);

                    if (author is null)
                    {
                        return TypedResults.NotFound();
                    }

                    context.Authors.Remove(author);
                    await context.SaveChangesAsync();

                    return TypedResults.NoContent();
                    */
                    var affected = await context.Authors.Where(a => a.Id == authorId)
                        .ExecuteDeleteAsync();

                    return affected == 1 ? TypedResults.NoContent() : TypedResults.NotFound();
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteAuthor")
            .WithOpenApi();
    }
}