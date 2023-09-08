namespace ConsoleApp1.Services;

public class ServiceLifetimeReporter
{
    private readonly IExampleTransientService _transientService;
    private readonly IExampleScopedService _scopedService;
    private readonly IExampleSingletonService _singletonService;
    private readonly IWhateverService _whateverService;

    public ServiceLifetimeReporter(IExampleTransientService transientService, IExampleScopedService scopedService,
        IExampleSingletonService singletonService, IWhateverService whateverService)
    {
        _transientService = transientService;
        _scopedService = scopedService;
        _singletonService = singletonService;
        _whateverService = whateverService;
    }

    public void ReportServiceLifetimeDetails(string lifetimeDetails)
    {
        Console.WriteLine(lifetimeDetails);

        _whateverService.LogHello();
        
        LogService(_transientService, "Always different");
        LogService(_scopedService, "Changes only with lifetime");
        LogService(_singletonService, "Always the same");
    }

    private static void LogService<T>(T service, string message)
        where T : IReportServiceLifetime =>
        Console.WriteLine(
            $"    {typeof(T).Name}: {service.Id} ({message})");
}