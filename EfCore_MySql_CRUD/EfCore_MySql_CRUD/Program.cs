using EfCore_MySql_CRUD.Infrastructure;
using EfCore_MySql_CRUD.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/***  Add Controllers ***/
// builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*** Add DbContext ***/
builder.Services.AddDbContext<PersonContext>(
        options => options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty));

/*** Dependencies injection ***/
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

var app = builder.Build();

/***  Map Controllers ***/
app.MapControllers();

/*** Use Swagger ***/
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet(("/"), (context) =>
{
    context.Response.Redirect("/swagger");
    return Task.FromResult(0);
});

app.Run();