using _2_WebHttpClientDemo.Model;
using Refit;

namespace _2_WebHttpClientDemo.HttpClientServices;

public interface IGeneratedHttpClientService
{
    [Get("/blogposts")]
    Task<BlogPost[]> GetBlogsAsync();
}