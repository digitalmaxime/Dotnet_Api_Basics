namespace Domain;

public class Author
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public ICollection<Book> Books { get; init; } = new List<Book>();
}

public class Book
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public Author Author { get; init; }
    public int AuthorId { get; init; }
    public Cover? Cover { get; init; }
}

public class Cover
{
    public int Id { get; init; }
    public string DesignIdeas { get; set; } = string.Empty;
    public int BookId { get; init; }
    public Book? Book { get; init; }
}