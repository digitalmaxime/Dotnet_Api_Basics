using _4_SqliteDatabase.Database;
using _4_SqliteDatabase.Endpoints;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogContext>();

var app = builder.Build();

app
    .MapBlogPostEndpoints()
    .MapBlogEndpoints();

app.Run();