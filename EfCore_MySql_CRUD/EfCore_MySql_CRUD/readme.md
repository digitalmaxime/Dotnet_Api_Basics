<h2>Run MySql Server with Docker</h2>

`docker run --name CrudDemo -e MYSQL_ROOT_PASSWORD=test123 --publish 1234:3306 -d mysql:latest`

<h2>Connect to MySql with Workbench client</h2>

<h2>EfCore Dependencies</h2>

``dotnet add package Microsoft.EntityFrameworkCore.Design``

``dotnet add package Microsoft.EntityFrameworkCore``

``dotnet add package  MySql.EntityFrameworkCore``

<h2>Swagger Dependencies</h2>

`dotnet add  package Swashbuckle.AspNetCore -v 6.2.3`

<h2>Create 4 layers (folders to make it simple)</h2>

- Api
- Application
- Infrastructure
- Domain

<h2>Api Controller</h2>

- Controller class must implement ControllerBase
- in program.cs (or serviceCollection extensions) *service* should `.AddControllers();`
- and *app* should `.MapControllers();`
- Add Swagger in program.cs 
    - `builder.Services.AddEndpointsApiExplorer();` 
    - `builder.Services.AddSwaggerGen();`
    - `app.UseSwagger();`
    - `app.UseSwaggerUI();`

<h2>Adding DbContext</h2>

Create a Context the implements `: DbContext`

`public PersonContext(DbContextOptions<PersonContext> options) : base(options)`

Link up entities

`public DbSet<Person> Persons => Set<Person>(); // Entity set for Entity 'Person'`

Register the context

``
builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
``

** *By default the lifetime of dbContext is 'scoped'*


<h2>Ef Core Fluent Mapping</h2>

```
public class PersonContext : DbContext
{
    // Specify DbSet properties etc
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().Property(t => t.Id).HasColumnName("PersonId");
    }
}
```

#### Separate Configuration Classes

```
public class PersonFluentMapping : IEntityTypeConfiguration<Person>
{

        public void Configure(EntityTypeBuilder<Person> builder)
        {
            throw new NotImplementedException();
        }
}
```

then add
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
  modelBuilder.ApplyConfiguration(new PersonFluentMapping());
}
```

or, simply add `ApplyConfigurationsFromAssembly` to get all implementations of IEntityTypeConfiguration

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
  modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}
```

ref : [link](https://www.learnentityframeworkcore.com/configuration/fluent-api)


<h2>Ef Core Migrations</h2>

Run ef migrations

`dotnet ef migrations add init`

`dotnet ef database update`