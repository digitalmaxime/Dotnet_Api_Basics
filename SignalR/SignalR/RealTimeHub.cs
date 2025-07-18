using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR;

public class RealTimeHub : Hub<IWeatherForecast>
{
    public async Task SendMessage(string message)
    {
        var forecast = Enumerable.Range(0, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55)))
            .ToArray();
        await Clients.All.WeatherUpdated(forecast);
    }
}

public interface IWeatherForecast
{
    Task WeatherUpdated(IEnumerable<WeatherForecast> forecast);
} 

public class WeatherUpdateService : BackgroundService
{
    private readonly IHubContext<RealTimeHub> _hubContext;
    private readonly ILogger<WeatherUpdateService> _logger;

    public WeatherUpdateService(IHubContext<RealTimeHub> hubContext, ILogger<WeatherUpdateService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var forecast = Enumerable.Range(0, 5).Select(index =>
                    new WeatherForecast
                    (
                        DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        Random.Shared.Next(-20, 55)))
                .ToArray();
            
            try
            {
                var message = $"Server time: {DateTime.Now}";
                await _hubContext.Clients.All.SendAsync("WeatherUpdated", forecast, stoppingToken);
                _logger.LogInformation("Message sent: {Message}", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}