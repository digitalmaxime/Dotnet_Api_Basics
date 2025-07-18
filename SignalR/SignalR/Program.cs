using Microsoft.AspNetCore.SignalR;
using SignalR;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") // Angular default dev port
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHostedService<WeatherUpdateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularPolicy");

app.UseDefaultFiles(); // helps serve the main `index.html` when users hit the root URL
app.UseStaticFiles(); // necessary to serve all Angular assets (JS, CSS, images)


app.MapGet("/api/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(0, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55)))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapHub<RealTimeHub>("/realtimehub");
app.MapControllers();


app.MapFallbackToFile("index.html"); // if nothing else matches this request, send back index.html and let Angular handle it

app.Run();