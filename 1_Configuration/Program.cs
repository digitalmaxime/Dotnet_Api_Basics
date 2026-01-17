using Microsoft.Extensions.Options;
using WebConfigurationDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ComplexSetting>()
    .Bind(builder.Configuration.GetRequiredSection("MyComplexSetting"));

builder.Services.AddHttpClient();

var app = builder.Build();

/*
* Different ways to access configuration settings
*/
Console.WriteLine(app.Configuration["MyCustomSetting"]);
Console.WriteLine(app.Configuration.GetValue<string>("MyCustomSetting"));
Console.WriteLine(app.Configuration.GetSection("MyCustomSetting").Value);
Console.WriteLine(app.Configuration.GetSection("MyCustomSetting").Get<string>());
Console.WriteLine(app.Configuration.GetRequiredSection("MyCustomSetting").Get<string>());

/* Access typed complex settings */
var myComplexSetting = app.Configuration.GetRequiredSection("MyComplexSetting").Get<ComplexSetting>();
Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(myComplexSetting));

/* default endpoint using options pattern to access complex settings */
app.MapGet("/", (IOptions<ComplexSetting> options) => "Hello World from WebConfigurationDemo!" +
        $"\n\tCustom Setting: {app.Configuration["MyCustomSetting"]}" +
        $"\n\tComplex Setting: {System.Text.Json.JsonSerializer.Serialize(options.Value)}"
);

app.MapGet("/blogposts", () => new[] { new { Title = "Post 1", Content = "Content 1" }, new { Title = "Post 2", Content = "Content 2" } });

app.MapPost("/blogposts", (BlogPost post) =>
{
    Console.WriteLine($"Processing blog post: {post.Title}");
    return $"Received post with title: {post.Title}";
});

app.Run();

internal class BlogPost
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}