using System.Reflection;
using _4_SqliteDatabase.Models;
using Microsoft.EntityFrameworkCore;

namespace _4_SqliteDatabase.Database;

public class BlogContext(DbContextOptions<BlogContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source=blog.db")
            .UseSeeding((context, _) => /* Runs on EnsureCreated, Migrate(), and ef database update. */
            {
                var testBlog = context.Set<Blog>().FirstOrDefault(b => b.Title == "Blog 3");
                
                if (testBlog != null) return;
                
                context.Set<Blog>().Add(new Blog { Title = "Blog 3", BlogPosts = new List<BlogPost>()
                {
                    new() { Title = "Post 1", Author = "Author 1", Content = "Blog 3 Post 1 Content" },
                    new() { Title = "Post 2", Author = "Author 2", Content = "Blog 3 Post 2 Content" }
                }});
                context.SaveChanges();
            });
        
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
}