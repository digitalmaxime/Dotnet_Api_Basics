namespace ConsoleApp1.Services;

public class ExampleScopedService : IExampleScopedService
{
    // public Guid Id { get => Guid.NewGuid(); }
    Guid IReportServiceLifetime.Id { get; } = Guid.NewGuid();

}