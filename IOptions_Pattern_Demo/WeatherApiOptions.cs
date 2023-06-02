using System.ComponentModel.DataAnnotations;

namespace IOptions_Pattern_Demo;

public class WeatherApiOptions
{
    public const string WeatherApi = "WeatherApi";
    
    [Required]
    public string Url { get; set; } 
    [Required]
    public string Key { get; set; }

    [Range(1, 100)]
    public int Count { get; set; }
}