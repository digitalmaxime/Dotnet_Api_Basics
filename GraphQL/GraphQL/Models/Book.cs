namespace GraphQL.Models;

public class Book
{
    public required string Title { get; set; }

    public required Author Author { get; set; }
}