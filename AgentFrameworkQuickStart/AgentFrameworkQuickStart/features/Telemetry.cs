using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AgentFrameworkQuickStart.features;

public static class Telemetry
{
    /*
     * Using Jaeger to view the telemetry data
     * Make sure you have Jaeger running locally.
     * Visit http://localhost:16686/ to view the telemetry data
     */
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Telemetry ---");

        // must match the tracerProvider source name and the chatClient's telemetry source name'
        const string agentTelemetrySource = "agent-telemetry-source";

        // Create a TracerProvider that exports to the OtlpExporter (open telemetry protocol)
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DemoAgent"))
            .AddSource(agentTelemetrySource)
            .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://localhost:4317"))
            .Build();

        var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        var chatClient = azureClient
            .GetChatClient(deploymentName)
            .CreateAIAgent(instructions: "You are good at telling jokes. about telemetry", name: "Joker")
            .AsBuilder()
            .UseOpenTelemetry(sourceName: agentTelemetrySource, configure: c => c.EnableSensitiveData = true)
            .Build();

        Console.WriteLine(await chatClient.RunAsync("Tell me a space exploration joke."));
        Console.WriteLine();
    }

}