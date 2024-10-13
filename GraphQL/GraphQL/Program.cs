using GraphQL.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

// Middleware

app.MapGraphQL();

app.MapGet("/", () => "Hello World!");


await app.RunAsync();
