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
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Person> Person => Set<Person>(); // Entity set for Entity 'Person'`
}