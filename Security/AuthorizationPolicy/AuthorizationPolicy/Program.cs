using System.Text;
using AuthorizationPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminScope", policy => {
        policy.RequireClaim("scope", "admin");
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapGet("/weatherforecast", Handler.GetWeatherForecastDelegate)
    .WithName("GetWeatherForecast")
    .RequireAuthorization("AdminScope")
    .WithOpenApi();

app.Run();