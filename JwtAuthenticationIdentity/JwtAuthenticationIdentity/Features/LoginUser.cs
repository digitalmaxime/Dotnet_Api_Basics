using System.Security.Claims;
using System.Text;
using JwtAuthenticationIdentity.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthenticationIdentity.Features;

public static class LoginUser
{
    public record Request(string Username, string Password);

    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var group = endpointRouteBuilder.MapGroup("authentications");

        group.MapPost("login",
            async Task<Results<Ok<string>, UnauthorizedHttpResult>> (IOptions<JwtOptions> option, Request request,
                UserManager<IdentityUser> userManager) =>
            {
                var user = await userManager.FindByNameAsync(request.Username);

                if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
                    return TypedResults.Unauthorized();

                var roles = await userManager.GetRolesAsync(user);

                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.Value.SigningKey));
                var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = [
                    new(JwtRegisteredClaimNames.Sub, user.Id),
                    ..roles.Select(role => new Claim(ClaimTypes.Role, role))
                ];

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(option.Value.ExpirationInMinutes),
                    SigningCredentials = credentials
                };
                
                var token = new JsonWebTokenHandler().CreateToken(tokenDescriptor);


                return TypedResults.Ok(token);
            });

        return endpointRouteBuilder;
    }
}