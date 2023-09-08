using ConsoleApp1;
using ConsoleApp1.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<IExampleTransientService, ExampleTransientService>();
builder.Services.AddScoped<IExampleScopedService, ExampleScopedService>();
builder.Services.AddSingleton<IExampleSingletonService, ExampleSingletonService>();
builder.Services.AddTransient<IWhateverService, WhaterverService>();
builder.Services.AddTransient<ServiceLifetimeReporter>(); // <<-- the service being used in program.cs ( "provider.GetRequiredService<ServiceLifetimeReporter>();" )

using IHost host = builder.Build();

ExemplifyServiceLifetime(host.Services, "Lifetime 1");
ExemplifyServiceLifetime(host.Services, "Lifetime 2");

// await host.RunAsync();


static void ExemplifyServiceLifetime(IServiceProvider hostProvider, string lifetime)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    ServiceLifetimeReporter logger = provider.GetRequiredService<ServiceLifetimeReporter>();
    logger.ReportServiceLifetimeDetails(
        $"{lifetime}: Call 1 to provider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine("...");

    logger = provider.GetRequiredService<ServiceLifetimeReporter>();
    logger.ReportServiceLifetimeDetails(
        $"{lifetime}: Call 2 to provider.GetRequiredService<ServiceLifetimeReporter>()");

    Console.WriteLine();
}


/********************************************************************** */

/*
var answer = AskUser();

while (answer != "no")
{
    if (answer == "yes")
    {
        ExemplifyServiceLifetime(host.Services, "Lifetime 3");
    }

    answer = AskUser();
}

Console.WriteLine("Bye bye..");
Console.WriteLine();

return;

static string AskUser()
{
    Console.WriteLine("Wanna trigger ExemplifyServiceLifetime? (yes, no)");
    var answer = Console.ReadLine();
    var currentDate = DateTime.Now;
    Console.WriteLine($"{Environment.NewLine}You chose {answer}, on {currentDate:d} at {currentDate:t}!");
    
    return answer ?? "";
}

*/