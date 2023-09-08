namespace HttpClient_Demo;

public interface IWeatherService
{
    Task<string> Get(string cityName);
}

public class WeatherService : IWeatherService
{
    private HttpClient _httpClient;
    
    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("New Weather service created!");
        Console.WriteLine();
        Console.WriteLine();
    }

    public async Task<string> Get(string cityName)
    {
        var apiKey = "64102e915c694dcfae4171920230106";
        string API_URL = $"?key={apiKey}&q={cityName}";
        var response = await _httpClient.GetAsync(API_URL);
        return await response.Content.ReadAsStringAsync();
    }
}