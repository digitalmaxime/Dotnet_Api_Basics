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
    private readonly CityOption _cityOption;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<WeatherApiOptions> weatherOptions,
        IOptionsSnapshot<CityOption> cityOption
        )
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _cityOption = cityOption.Value;
        _weatherOptions = weatherOptions.Value;

        Console.WriteLine($"CITY OPTION 1 : {_cityOption.City1.Name}");
        Console.WriteLine($"CITY OPTION 2 : {_cityOption.City2.Name}");
        Console.WriteLine($"CITY OPTION 3 : {_cityOption.City3.Name}");
    }

    public IOptionsSnapshot<CityOption> CityOption { get; }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get(string cityName)
    {
        var baseUrl = _weatherOptions.Url;
        string? key = _weatherOptions.Key;
        
        using var toto = _cityOption.GetEnumerator();
        foreach (var city in _cityOption)
        {
            var c = city;
        }
        
        var url = $"{baseUrl}?key={key}&q={cityName}&aqi=no";
        Console.WriteLine($"\n\n\nUrl : {url}\n\n\n");
        using var httpClient = _httpClientFactory.CreateClient();
        return await httpClient.GetStringAsync(url);
    }
}
