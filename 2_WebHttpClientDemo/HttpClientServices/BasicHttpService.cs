using System.Text.Json;
using _2_WebHttpClientDemo.Model;

namespace _2_WebHttpClientDemo.HttpClientServices;

public class BasicHttpService(IHttpClientFactory httpClientFactory)
{
    public const string Name = "BasicHttpService";
    public async Task<BlogPost[]> GetBlogsAsync()
    {
        var client = httpClientFactory.CreateClient();

        try
        {
            var blogPosts = await client.GetFromJsonAsync<BlogPost[]>(
                $"https://localhost:8080/blogposts",
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return blogPosts ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return [];
        }
    }
}