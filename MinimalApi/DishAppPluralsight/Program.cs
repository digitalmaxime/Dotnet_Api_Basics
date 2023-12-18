using DishAppPluralsight.DbContexts;
using DishAppPluralsight.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Configure Services
builder.Services.AddAutoMapper(assemblies: AppDomain.CurrentDomain.GetAssemblies());
/*** Add DbContext ***/
builder.Services.AddDbContext<DishesDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

var app = builder.Build();
// Configure app (http pipeline and routes)

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

// Recreate and migrate database on each run, for demo purposes
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();