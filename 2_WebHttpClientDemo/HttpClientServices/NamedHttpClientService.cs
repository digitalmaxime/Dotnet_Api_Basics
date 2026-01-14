using System.Text.Json;
using _2_WebHttpClientDemo.Model;

namespace _2_WebHttpClientDemo.HttpClientServices;

public class NamedHttpClientService(IHttpClientFactory httpClientFactory)
{
    public const string Name = "NamedClientService";
    public async Task<BlogPost[]> GetBlogsAsync()
    {

        var client = httpClientFactory.CreateClient("NamedClient");
        try
        {
            var todos = await client.GetFromJsonAsync<BlogPost[]>("/blogposts");

            return todos ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return [];
        }
    }
}