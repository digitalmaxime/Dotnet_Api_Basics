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
            tools: [AIFunctionFactory.Create(Tools.GeneralTools.GetDateTime)]
        )
            .AsBuilder()
            .Use(CustomFunctionCallingMiddleware)
            .Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
            .Build();

        Console.WriteLine(await agent.RunAsync("What time is it?"));
        Console.WriteLine();

        return agent;
    }


    /* IChatClient middleware */
    private static async Task<ChatResponse> CustomChatClientMiddleware(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options,
        IChatClient innerChatClient,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"\tIChatClient middleware before");
        var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
        Console.WriteLine($"\tIChatClient middleware after : about to be deliver : {response.Messages.Last()}");

        return response;
    }
    
    /* Function calling middleware */
    private static async ValueTask<object?> CustomFunctionCallingMiddleware(
        AIAgent agent,
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"\t\tFunction calling middleware before - Function Name: {context!.Function.Name}");
        var result = await next(context, cancellationToken);
        Console.WriteLine($"\t\tFunction calling middleware after - Function Call Result: {result}");

        return result;
    }
    
    /* Agent Run Middleware */
    private static async Task<AgentResponse> CustomAgentRunMiddleware(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        AIAgent innerAgent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Agent Run Middleware before");
        var response = await innerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"Agent Run Middleware after\n");
        return response;
    }
    
}