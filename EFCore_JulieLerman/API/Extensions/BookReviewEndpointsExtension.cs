using Application.FeatureBookReview;
using Application.FeatureBookReview.Command;
using Application.FeatureBookReview.Query;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EFCore_JulieLerman.Extensions;

internal static class BookReviewEndpointsExtension
{
    internal static void MapBookReviewEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("api/bookreview").WithTags("bookReview");
        
        group.MapPost("",
                async Task<Results<Ok<BookReviewCommandResponse>, BadRequest>> 
                (BookReviewCommand command, IMediator mediator) =>
                {
                    var response = await mediator.Send(command);
                    return TypedResults.Ok(response);
                })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("CreateBookReview")
            .WithOpenApi();

        group.MapGet("/{bookTitle}",
            async Task<Results<Ok<BookReviewQueryResponse>, NotFound>> (string bookTitle, IMediator mediator) =>
            {
                var response = await mediator.Send(new BookReviewQuery(bookTitle));
                return response == null ? TypedResults.NotFound() : TypedResults.Ok(response);
            });
    }
}