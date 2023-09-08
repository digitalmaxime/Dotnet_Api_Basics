using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1.Services;

internal sealed class ExampleSingletonService : IExampleSingletonService
{
    // public Guid Id { get; } = Guid.NewGuid();
    Guid IReportServiceLifetime.Id { get; } = Guid.NewGuid();

    public ServiceLifetime Lifetime { get => ServiceLifetime.Singleton; }
}