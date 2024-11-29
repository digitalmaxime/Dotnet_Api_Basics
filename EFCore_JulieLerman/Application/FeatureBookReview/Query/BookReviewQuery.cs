using MediatR;

namespace Application.FeatureBookReview.Query;

public record BookReviewQuery(string BookTitle): IRequest<BookReviewQueryResponse?>;