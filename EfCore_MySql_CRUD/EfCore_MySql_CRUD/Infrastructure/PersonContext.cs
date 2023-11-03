using System.Reflection;
using EfCore_MySql_CRUD.Domain;
using Microsoft.EntityFrameworkCore;

namespace EfCore_MySql_CRUD.Infrastructure;

public class PersonContext: DbContext
{
    public PersonContext(DbContextOptions<PersonContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // modelBuilder.ApplyConfiguration(new PersonFluentMapping());
        // modelBuilder.Entity<Person>().HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Person> Person => Set<Person>(); // Entity set for Entity 'Person'`
}