# EF Core

Its an ORM _Object Relation Mapper_

which relies on model

POCOS _plain old CLR objects_
- class with no base class or interface

## Implementation steps
1) [Define the model classes](#model)
2) [Add required pacakges](#packages)
3) [Create the database context and DbSets](#context)
4) [Overriding the conventions with FluentApi](#FluentApi)
5) [Configure the application to use EF Core](#configure)
6) [Add connection string](#Connection-String)
7) [Migrations](#migrations)
7) [View database](#database)

## Packages
- `Microsoft.EntityFrameworkCore.SqlServer` includes the `Microsoft.EntityFrameworkCore` package itself
- `Microsoft.EntityFrameworkCore.Design` why not mentioned by Gill?? for migrations
- `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` contains types that will be useful to capture and report diagnostics from EF Core

Dotnet CLI `dotnet add package <packageName>`

## Context

Context sits between app and database

Defined in the `/models` folder if the project is super simple or..

Defined in the `/Infra/Persistence/Contexts` otherwise

- class that derives from DbContext base class
- represents a session with database
- allows
    - connection
    - model building
    - change tracking
    - querying

E.g. `public class MyDbCtx : DbContext `
- `public ctor(DbContextOptions<MyDbContext> options) : base(options)`
- `{...}`
- `public DbSet<Categroy> Categories { get; set; }` 

*notice that the table name is plural*

*note that the `<Category>` type will define the Categories table structure*


## Model

#### EF Core Conventions

table names
- should be plural e.g. `public DbSet<Category> Categories { get; set; }`

Colomn names
- Created columns will match the property names

Primary key
- `Id` or `<EntityName>Id` become primary key

Foreign keys
- `class Pie { ...`
- `--> public int CategoryId { get; set; } // ForeignKey`
- `--> public Category? Category { get; set; } // navigation prop`
- `}`
- Convention : `<navigationPropName>Id` (+ `<navigationPropName>` optional) is treated as foreign key

Types
- string --> nvarchar(max)
- Non-nullable --> required

## FluentApi

Overriding Conventions with Fluent Api
- can influence the behavior of the aformentioned conventions

#### Overriding the created tables

E.g. `public class MyDbCtx : DbContext `
- `protected override voir OnModelCreating(ModelBuilder modelBuilder)`
- `{ modelBuilder.Entity<Pie>().ToTable("Pie"); }`

#### Overriding the attributes

E.g. `public class MyDbCtx : DbContext {` 
```
protected override voir OnModelCreating(ModelBuilder modelBuilder)
{ 
    modelBuilder.Entity<Category>()
    .Property(b => b.Name).IsRequired(); // fluent api
}   
```
`}`


## Configure

Application configuration in services collection

In the `program.cs` the dbContext is registered as scoped through `.AddDbContext<>()`.

The Change tracker associated with de DbContext is therefore also scoped (per request basis).

``` 
/program.cs

// Add DbContext to the services collection in a scoped way, allowing  DI
builder.Services.AddDbContext<BethanysPieShopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BethanysPieShopDbContextConnection"));
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
```

In reading scenarios, use `AsNoTracking`.

## Connection-String

```
  "ConnectionStrings": {
    "BethanysPieShopDbContextConnection": "Server=(localdb)\\mssqllocaldb;Database=BethanysPieShop;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
```

## Migrations

CLI `dotnet ef migrations add` and  `dotnet ef database update`

## Database

to connect to localdb use `(localdb)\MSSQLLocalDBwith` Windows Auth