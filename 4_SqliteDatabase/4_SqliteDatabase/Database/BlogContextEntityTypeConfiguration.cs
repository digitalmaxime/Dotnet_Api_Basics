using _4_SqliteDatabase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _4_SqliteDatabase.Database;

public class BlogContextEntityTypeConfiguration : IEntityTypeConfiguration<Blog>
{
    public void Configure(EntityTypeBuilder<Blog> builder)
    {
        builder.Property(b => b.Title).HasMaxLength(50);
        builder.HasData(new List<Blog>()
        {
            new Blog()
            {
                BlogId = 1, // Explicitly specifying the PK value
                Title = "Blog 1",
            },
            new Blog()
            {
                BlogId = 2,
                Title = "Blog 2",
            }
        });
    }
}