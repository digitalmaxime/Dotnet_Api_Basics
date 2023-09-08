using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApp1;

public interface IReportServiceLifetime
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}
