using Application.FeatureBookReview.Services;
using MediatR;

namespace Application.FeatureBookReview.Query;

public class BookReviewQueryHandler: IRequestHandler<BookReviewQuery, BookReviewQueryResponse?>
{
    private readonly IBookReviewService _bookReviewService;

    public BookReviewQueryHandler(IBookReviewService bookReviewService)
    {
        _bookReviewService = bookReviewService;
    }
    
    public async Task<BookReviewQueryResponse?> Handle(BookReviewQuery request, CancellationToken cancellationToken)
    {
        var bookReview = await _bookReviewService.GetBookReview(request.BookTitle);
        return bookReview == null ? null : new BookReviewQueryResponse { Message = bookReview };
    }
}