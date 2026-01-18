namespace _4_SqliteDatabase.Models;

public class Blog
{
    public int BlogId { get; set; }
    public required string Title { get; set; }
    public List<BlogPost> BlogPosts { get; set; } = [];
}