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
    
    public async Task<string> CreatePostAsyn(string title, string content)
    {
        var client = httpClientFactory.CreateClient();

        try
        {
            var response = await client.PostAsJsonAsync<BlogPost>(
                $"https://localhost:8080/blogposts",
                new BlogPost { Title = title, Content = content },
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return "error";
        }
    }
}