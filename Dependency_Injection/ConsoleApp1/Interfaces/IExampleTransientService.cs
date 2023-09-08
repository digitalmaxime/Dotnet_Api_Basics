using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1;

public interface IExampleTransientService : IReportServiceLifetime
{
    ServiceLifetime IReportServiceLifetime.Lifetime => ServiceLifetime.Transient;
}