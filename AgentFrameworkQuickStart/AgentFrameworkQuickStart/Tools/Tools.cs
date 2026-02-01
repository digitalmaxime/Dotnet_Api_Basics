using System.ComponentModel;

namespace AgentFrameworkQuickStart.Tools;

public class GeneralTools
{
    private const string UtcDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

    [Description("Get the weather for a given location.")]
    public static string GetWeather([Description("The location to get the weather for.")] string location)
        => $"The weather in {location} is cloudy with a high of 15°C.";
    
    [Description("Get the current utc date and time.")]
    public static string GetDateTime()
        => $"The current utc date time is {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss}";
}