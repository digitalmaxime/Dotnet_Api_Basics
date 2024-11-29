using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Persistence.Extensions;

namespace Persistence;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PersistenceLibrary).Assembly);
        modelBuilder.SeedData();
    }
    
    public override int SaveChanges()
    {
        foreach (var entity in ChangeTracker.Entries<Author>())
        {
            entity.Property("LastUpdated").CurrentValue = DateTimeOffset.Now;
        }
        return base.SaveChanges();
    }
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Cover> Covers { get; set; }
    public DbSet<Author> Authors { get; set; }
    
}