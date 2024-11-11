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
    
    
    public DbSet<Book> Books { get; set; }
    public DbSet<Cover> Covers { get; set; }
    public DbSet<Author> Authors { get; set; }
    
}