using MediatR;

namespace Application.FeatureBookReview.Command;

public class BookReviewCommandHandler: IRequestHandler<BookReviewCommand, BookReviewCommandResponse>
{
    public Task<BookReviewCommandResponse> Handle(BookReviewCommand request, CancellationToken cancellationToken)
    {
        var response = new BookReviewCommandResponse()
        {
            Message = "Book review has been successfully added."
        };

        return Task.FromResult(response);
    }
}