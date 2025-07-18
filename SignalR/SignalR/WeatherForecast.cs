record WeatherForecast(DateOnly Date, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
    public string Condition { get; } = WeatherForecastData.Summary;
}

record WeatherForecastData
{
    
    private static readonly string[] AllSummaries = new[]
    {
        "sunny", "cloudy", "rainy"
    };
    
    public static string Summary => AllSummaries[Random.Shared.Next(AllSummaries.Length)];
}
