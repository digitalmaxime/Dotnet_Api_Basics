using Microsoft.AspNetCore.Mvc.Testing;

namespace Test.ApiTest;

public class ApiIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/api/author")]
    [InlineData("/api/author/1")]
    public async Task TestGetAuthor(string url)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }

    [Theory]
    [InlineData("/api/bookreview/Book 123")]
    public async Task TestGetBook(string url)
    {
        // Arrange

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType.ToString());
    }
}