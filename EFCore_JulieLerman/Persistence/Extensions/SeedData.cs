using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Extensions;

public static class SeedDataExtension
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>().HasData(new
        {
            Id = 1,
            Name = "Author 1",
            LastUpdated = DateTimeOffset.Now
        });

        modelBuilder.Entity<Book>().HasData(new List<Book>()
        {
            new Book()
            {
                Id = 1,
                Title = "Book 123",
                AuthorId = 1
            }
        });

        modelBuilder.Entity<Cover>().HasData(new List<Cover>()
        {
            new Cover()
            {
                Id = 1,
                DesignIdeas = "Design ideas for Book 1",
                BookId = 1,
                DesignIdeaArray = new List<string> { "Idea 1", "Idea 2", "Idea 3" }
            }
        });
    }
}