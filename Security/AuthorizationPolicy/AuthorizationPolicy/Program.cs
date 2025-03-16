using System.Text;
using AuthorizationPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        /*
        *  "dotnet user-jwts create" encodes the signin key in base64
        */
        byte[] secretKeyBytes = Convert.FromBase64String("yBciNkH+sX6KOCTEHQOCTjgQ26YrP+olt2RJuDCE1Rc=");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminScope", policy =>
    {
        policy.RequireClaim("scope", "admin");
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", Handler.GetWeatherForecastDelegate)
    .WithName("GetWeatherForecast")
    .RequireAuthorization("AdminScope")
    .WithOpenApi();

app.Run();