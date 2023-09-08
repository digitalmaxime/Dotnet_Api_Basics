using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1;

public interface IExampleScopedService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Scoped;
}