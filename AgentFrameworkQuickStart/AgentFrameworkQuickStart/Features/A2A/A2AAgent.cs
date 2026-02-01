using System.Net.Http.Json;
using A2A;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Agents.AI.Hosting.A2A;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Microsoft.AspNetCore.OpenApi;

namespace AgentFrameworkQuickStart.Features.A2A;

public class A2AAgent
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- A2A Agent ---");
        var pizzaAgentEndpoint = "http://localhost:5001";

        /*
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(pizzaAgentEndpoint);
        var agentCard = await httpClient.GetAsync("/a2a/pizza/v1/card");

        // Build A2A request
        var a2aRequest = new

        {
            message =  new {
            kind = "message",
            role = "user",
            parts =
            (List<object>)[ new 
                {
                    kind = "text",
                    text = "Order me a pizza",
                    metadata = new {}
                }
            ],
            messageId = "messageid-123",
            contextId = "foo"
        }
        };

        var response = await httpClient.PostAsJsonAsync("http://localhost:5001/a2a/pizza/v1/message:send", a2aRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
        */

        A2ACardResolver agentCardResolver = new(new Uri("https://your-a2a-agent-host"));
        AIAgent agent = await agentCardResolver.GetAIAgentAsync();
    }
}