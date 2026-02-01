using System.Text.Json;
using _2_WebHttpClientDemo.Model;

namespace _2_WebHttpClientDemo.HttpClientServices;
/*
* In order for DI to work '
*    Services.AddHttpClient<TypedHttpClient>(...);
*    ([FromServices] TypedHttpClient lient) => await client.GetBlogsAsync())
* class 'TypedHttpClient' must have a constructor with parameter HttpClient
*/
public class TypedAndResilientHttpClient(HttpClient httpClient) 
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
    
    
    public async Task<string> CreatePostAsyn(string title, string content)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync<BlogPost>(
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