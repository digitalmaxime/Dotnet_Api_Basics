using Application.FeatureBookReview.Services;
using Domain;
using NSubstitute;
using Persistence.Repositories;

namespace Test.UnitTest;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var builder = new ServiceBuilder();
        var service = builder.Build();
        
        var response = await service.GetBookReview("Test Title");
        Assert.NotNull(response);
        Assert.Equal("The book Test Title was pretty bad in fact", response);
    }

    private class ServiceBuilder
    {
        private readonly IBookRepository _bookRepositoryMock =  Substitute.For<IBookRepository>();
        public BookReviewService Build()
        {
            _bookRepositoryMock.GetBookByTitleAsync(Arg.Any<string>()).Returns(new Book() { Title = "Test Title" });
            return new BookReviewService(_bookRepositoryMock);
        }
    }
}