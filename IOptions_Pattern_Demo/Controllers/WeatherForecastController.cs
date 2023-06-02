using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IOptions_Pattern_Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly WeatherApiOptions _weatherOptions;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<WeatherApiOptions> weatherOptions)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _weatherOptions = weatherOptions.Value;
    }
    
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get(string cityName)
    {
        var baseUrl = _weatherOptions.Url;
        string? key = _weatherOptions.Key;
        
        var url = $"{baseUrl}?key={key}&q={cityName}&aqi=no";
        Console.WriteLine($"\n\n\nUrl : {url}\n\n\n");
        using var httpClient = _httpClientFactory.CreateClient();
        return await httpClient.GetStringAsync(url);
    }
}
