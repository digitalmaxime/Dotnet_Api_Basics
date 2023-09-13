using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using AppSettingsDemo.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();



/****************************************************************/

// ***************** Reading Configuration AppSettings

var validIssuerConfig = builder.Configuration.GetValue<string>("Security:ValidIssuer");
Console.WriteLine("---------------");
Console.WriteLine(validIssuerConfig);
Console.WriteLine("---------------");


var myKeyValue = builder.Configuration["MyKey"];
var title = builder.Configuration["Position:Title"];
var name = builder.Configuration["Position:Name"];
var defaultLogLevel = builder.Configuration["Logging:LogLevel:Default"];
Console.WriteLine($"MyKey value: {myKeyValue} \n" +
                  $"Title: {title} \n" +
                  $"Name: {name} \n" +
                  $"Default Log Level: {defaultLogLevel}");


// ************ Reading Configuration AppSettings Options Pattern

var positionOptions = new PositionOptions();
builder.Configuration.GetSection(PositionOptions.Position).Bind(positionOptions);

Console.WriteLine($"Title: {positionOptions.Title} \n" +
                  $"Name: {positionOptions.Name}");

positionOptions = builder.Configuration.GetSection(PositionOptions.Position)
    .Get<PositionOptions>();

Console.WriteLine($"Title: {positionOptions.Title} \n" +
                  $"Name: {positionOptions.Name}");

// Allow to dependency inject ctor(IOptions<PositionOptions> options) --> 
builder.Services.Configure<PositionOptions>(
    builder.Configuration.GetSection(PositionOptions.Position));


/****************************************************************/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
