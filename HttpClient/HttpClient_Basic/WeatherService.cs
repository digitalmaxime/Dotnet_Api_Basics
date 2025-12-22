namespace HttpClient_Demo;

public interface IWeatherService
{
    Task<string> Get(string cityName);
}

public class WeatherService : IWeatherService
{
    private HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("New Weather service created!");
        Console.WriteLine();
        Console.WriteLine();
    }

    public async Task<string> Get(string cityName)
    {
        var apiKey = _configuration.GetSection("apiKey").Get<string>();
        string API_URL = $"?key={apiKey}&q={cityName}";
        var response = await _httpClient.GetAsync(API_URL);
        return await response.Content.ReadAsStringAsync();
    }
}