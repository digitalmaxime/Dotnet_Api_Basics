using JwtAuthenticationIdentity.Data;
using JwtAuthenticationIdentity.Models;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthenticationIdentity.Features;

public static class RegisterUser
{
    public static IEndpointRouteBuilder MapUserRegistrationEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("authentications");
        group.MapPost("register", async ([FromBody] Person person, AuthenticationDbContext context) =>
        {

            context.Add(person);
            
            return TypedResults.Ok();
        });
        
        return endpointRouteBuilder;
    }
}