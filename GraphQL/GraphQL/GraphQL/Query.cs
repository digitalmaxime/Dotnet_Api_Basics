using GraphQL.Models;

namespace GraphQL.GraphQL;

/**
 * This class is a query class that will be used to query the data from the GraphQL server.
 * The GetBook method returns a new instance of the Book class with the title "C# in depth." and the author "Jon Skeet"
 * The field in question is called GetBook, but the name will be shortened to just book in the resulting schema.
 */

public class Query
{
    public Book GetBook() =>
        new Book
        {
            Title = "C# in depth.",
            Author = new Author
            {
                Name = "Jon Skeet"
            }
        };
}
