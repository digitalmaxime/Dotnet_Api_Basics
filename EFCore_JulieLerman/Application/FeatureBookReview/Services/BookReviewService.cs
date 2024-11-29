using Persistence.Repositories;

namespace Application.FeatureBookReview.Services;

public interface IBookReviewService
{
    void CreateBookReview();
    Task<string?> GetBookReview(string bookTitle);
}

public class BookReviewService : IBookReviewService
{
    private readonly IBookRepository _bookRepository;

    public BookReviewService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public void CreateBookReview()
    {
        
    }
    
    public async Task<string?> GetBookReview(string bookTitle)
    {
        var book = await _bookRepository.GetBookByTitleAsync(bookTitle);
        return book == null ? null : $"The book {book.Title} was pretty bad in fact";
    }
}