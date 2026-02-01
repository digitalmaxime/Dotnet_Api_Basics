using System.Net.Http.Json;
using A2A;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Logging;

namespace AgentFrameworkQuickStart.Features.A2A;

public class A2AAgent
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- A2A Agent ---");

        var pizzaAgentBaseUrl = "http://localhost:5001";
        A2ACardResolver agentCardResolver = new(new Uri(pizzaAgentBaseUrl));
        // Get the remote A2A agent (automatically discovers from /.well-known/agent.json)
        AIAgent pizzaAgent = await agentCardResolver.GetAIAgentAsync();
        Console.WriteLine($"Connected to remote agent: {pizzaAgent.Name}");
        // Example: Interact directly with the remote pizza agent
        // var response = await pizzaAgent.RunAsync("I'd like to order a large pepperoni pizza");


        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName).AsIChatClient();


        var chatClientAgentOptions = new ChatClientAgentOptions
        {
            Name = "Helpful Agent",
            Description = "Delegates pizza ordering to a remote pizzaAgent when needed.",
            ChatOptions = new ChatOptions
            {
                Instructions =
                    """
                    You are a helpful assistant.
                    If the user wants to order pizza, call the pizzaAgent tool.
                    Dont ask for clarifying question.
                    """,
                ToolMode = ChatToolMode.Auto,
                Tools = [ pizzaAgent.AsAIFunction()]
            },
            
        };

        var agent = new ChatClientAgent(
            client,
            options: chatClientAgentOptions
        );

        var session = await agent.GetNewSessionAsync();
        var response0 = await agent.RunAsync("Call OrderPizza with ‘large pepperoni’ now.");
        do
        {
            Console.WriteLine(">");
            var input = Console.ReadLine();
            if (input == "exit" || string.IsNullOrEmpty(input)) break;
            var response = await agent.RunAsync(input, session);
            Console.WriteLine(response);
        } while (true);


        // Console.WriteLine(await agent.RunAsync("How are you ?"));
        Console.WriteLine();
    }
}

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