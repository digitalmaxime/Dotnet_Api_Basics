using System.Collections;
using System.Globalization;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkQuickStart.Features;

public class MiddlewareAgent
{
    public static async Task<AIAgent> Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Middleware Agent ---");
    
        IChatClient client = new ChatClientBuilder(
                new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey)).GetChatClient(deploymentName)
                    .AsIChatClient())
            .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
            .Build();

        AIAgent agent = client.CreateAIAgent(
                name: "MiddlewareAgent",
                instructions:
                "You are a utility assistant that can redirect to existing agents. When asked for this information, use your available tools.",
                tools: []
            );
    
        Console.WriteLine(await agent.RunAsync("Tell me a what day it is in a surprising way"));
        Console.WriteLine();

        return agent;
    }
    
    
    static async Task<ChatResponse> CustomChatClientMiddleware(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options,
        IChatClient innerChatClient,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Request Message Count: {messages.Count()}");
        var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
        Console.WriteLine($"Response Message Count: {response.Messages.Count}");

        return response;
    }
}