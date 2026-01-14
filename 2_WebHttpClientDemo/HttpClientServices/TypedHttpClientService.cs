using _2_WebHttpClientDemo.Model;

namespace _2_WebHttpClientDemo.HttpClientServices;

public class TypedHttpClientService(HttpClient httpClient)
{
    public const string Name = "NamedClientService";
    public async Task<BlogPost[]> GetBlogsAsync()
    {
        try
        {
            var todos = await httpClient.GetFromJsonAsync<BlogPost[]>("/blogposts");

            return todos ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return [];
        }
    }
}