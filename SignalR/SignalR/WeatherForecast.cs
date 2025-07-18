namespace SignalR;

public record WeatherForecast(DateOnly Date, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
    public string Condition { get; } = WeatherForecastData.Summary;
}

internal abstract record WeatherForecastData
{
    
    private static readonly string[] AllSummaries =
    [
        "sunny", "cloudy", "rainy"
    ];
    
    public static string Summary => AllSummaries[Random.Shared.Next(AllSummaries.Length)];
}