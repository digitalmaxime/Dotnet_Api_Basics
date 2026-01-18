namespace _4_SqliteDatabase.Models;

public class BlogPost
{
    public int BlogPostId { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Content { get; set; }
}