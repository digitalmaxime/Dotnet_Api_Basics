using MediatR;

namespace Application.FeatureBookReview.Command;

public class BookReviewCommand: IRequest<BookReviewCommandResponse>
{
    public required string BookTitle { get; set; }
    public required string Review { get; set; }
}