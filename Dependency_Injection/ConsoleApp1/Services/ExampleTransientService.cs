namespace ConsoleApp1.Services;

public class ExampleTransientService : IExampleTransientService
{
    // public Guid Id { get => Guid.NewGuid(); }
    Guid IReportServiceLifetime.Id { get; } = Guid.NewGuid();

}