using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AgentFrameworkQuickStart.Features;

public class MiddlewareAgent
{
    public static async Task<AIAgent> Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Middleware Agent ---");

        var client = new ChatClientBuilder( // this is an "IChatClient" not ChatClient
                new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
                    .GetChatClient(deploymentName)
                    .AsIChatClient())
            .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
            .Build();

        var client2 = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .Use(getResponseFunc: CustomChatClientMiddleware, getStreamingResponseFunc: null)
            .Build();

        AIAgent agent = new ChatClientAgent(
            chatClient: client,
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
        Console.WriteLine($"Response Message about to be delivered : {response.Messages.Last()}");
        Console.WriteLine($"Response Message Count: {response.Messages.Count}");

        return response;
    }
}