using Application.FeatureBookReview.Services;
using Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;

namespace Test.InMemory;

public class InMemoryTest
{ 
    private static LibraryContext GetLibraryContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        var contextOptions = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlite(connection)
            .Options;
        connection.Open();
        var testDbContext = new LibraryContext(contextOptions);
        testDbContext.Database.EnsureCreated();
        testDbContext.Books.Add(new Book { Title = "Test Title", AuthorId = 1});
        testDbContext.SaveChanges();
        return testDbContext;
    }
    
    [Fact]
    public async Task Test1()
    {
        await using var context = GetLibraryContext();
        var builder = new ServiceBuilder(context);
        var service = builder.Build();
        var response = await service.GetBookReview("Test Title");
        Assert.NotNull(response);
        Assert.Equal("The book Test Title was pretty bad in fact", response);
    }
    
    private class ServiceBuilder
    {
        private readonly IBookRepository _bookRepository;
        
        public ServiceBuilder(LibraryContext testDbContext)
        {
            _bookRepository = new BookRepository(testDbContext);
        }
        public BookReviewService Build()
        {
            return new BookReviewService(_bookRepository);
        }
    }
}