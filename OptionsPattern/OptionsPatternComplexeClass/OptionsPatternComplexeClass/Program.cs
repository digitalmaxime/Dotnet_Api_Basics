using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OptionsPatternComplexeClass;
using OptionsPatternComplexeClass.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IRequestHandler, RequestHandler>();

var httpClientOptions = builder.Configuration
    .GetSection("OAuth2")
    .Get<HttpClientSecurityOption>();

builder.Services
    .Configure<HttpClientSecurityOption>(builder.Configuration.GetSection(HttpClientSecurityOption.Name));

builder.Services.AddOptions<SystemsOption>()
    .Bind(builder.Configuration.GetSection(SystemsOption.Name));

var app = builder.Build();

app.MapGet("", (IRequestHandler requestHandler) => { return requestHandler.ListAllHttpClientOptions(); });

app.MapGet("systems", (IRequestHandler requestHandler) => { return requestHandler.ListAllSystemOptions(); });

app.Run();