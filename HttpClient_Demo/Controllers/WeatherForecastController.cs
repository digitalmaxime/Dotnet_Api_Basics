using Microsoft.AspNetCore.Mvc;

namespace HttpClient_Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherService _weatherService;
    private static HttpClient? _httpClient;
    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IWeatherService weatherService
        )
    {
        _logger = logger;
        _weatherService = weatherService;
    }

    
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get(string cityName)
    {
        return await _weatherService.Get(cityName);
    }

}
