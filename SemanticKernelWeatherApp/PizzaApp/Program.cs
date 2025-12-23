using PizzaApp.endpoints;
using PizzaApp.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPizzaService, PizzaService>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.MapPizzaEndpoints();

app.Run();