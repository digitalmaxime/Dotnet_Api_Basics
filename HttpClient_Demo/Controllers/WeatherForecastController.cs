using Microsoft.AspNetCore.Mvc;

namespace HttpClient_Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private static HttpClient? _httpClient;
    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IHttpClientFactory httpClientFactory
        )
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get(string cityName)
    {
        var URL = $"?key=64102e915c694dcfae4171920230106&q={cityName}&aqi=no";

        var httpClient = _httpClientFactory.CreateClient("weather-consumer");
        var response = await httpClient.GetAsync(URL);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(response.Headers.Location);
        Console.WriteLine();
        Console.WriteLine();
        return await response.Content.ReadAsStringAsync();
    }

}
