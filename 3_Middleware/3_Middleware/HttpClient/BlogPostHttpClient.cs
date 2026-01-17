namespace _3_Middleware.HttpClient;

public class BlogPostHttpClient(System.Net.Http.HttpClient client, ILogger<BlogPostHttpClient> logger)
{
    public async Task<string> GetBlog()
    {
        return "Blog Post";
    }
}