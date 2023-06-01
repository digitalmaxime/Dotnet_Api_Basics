using Microsoft.AspNetCore.Mvc;

namespace HttpClient_Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private static HttpClient? _httpClient;
    public WeatherForecastController(
        ILogger<WeatherForecastController> logger
        )
    {
        _logger = logger;
    }

    // static WeatherForecastController()
    // {
    //     _httpClient = new HttpClient();
    // }
    
    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<string> Get(string cityName)
    {
        var URL = $"http://api.weatherapi.com/v1/current.json?key=64102e915c694dcfae4171920230106&q={cityName}&aqi=no";
        
        /*** Basic Solution that may lead to Socket exhaustion problem, no more httpClient may be create after a while ***/
        // using (var httpClient = new HttpClient())
        // {
        //     var response = await httpClient.GetAsync(URL);
        //     return await response.Content.ReadAsStringAsync();
        // }

        var response = await GetHttpClient().GetAsync(URL);
        return await response.Content.ReadAsStringAsync();
    }

    public static HttpClient GetHttpClient()
    {
        if (_httpClient == null)
        {
            _httpClient = new HttpClient();
            Console.WriteLine("\n\n\n Created a new HttpClient\n\n\n");
        }
    
        return _httpClient;
    }
}
