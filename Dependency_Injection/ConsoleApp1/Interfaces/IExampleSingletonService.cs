using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1;

public interface IExampleSingletonService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Singleton;
}