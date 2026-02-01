using JwtAuthenticationIdentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationIdentity.Data;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : IdentityDbContext(options)
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=authentication.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Person>()
            .HasData(new Person {Id = 1, Name = "Alice", Email = ""}, new Person {Id = 2, Name = "Bob", Email = ""});

        // modelBuilder.HasDefaultSchema("AspNetUsers");
        base.OnModelCreating(modelBuilder);
    }
    
    public DbSet<Person> Persons { get; set; }
}