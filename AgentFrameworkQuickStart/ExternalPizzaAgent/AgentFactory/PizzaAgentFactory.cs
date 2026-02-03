using A2A;
using Azure;
using Azure.AI.OpenAI;
using ExternalPizzaAgent.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace ExternalPizzaAgent.AgentFactory;

public class PizzaAgentFactory
{
    public const string PizzaAgentName = "PizzaAgent";

    public static AgentCard CreateAgentCard(string baseUrl)
    {
        return new AgentCard
        {
            Name = PizzaAgentName,
            Description = "An agent that manage pizza ordering",
            Version = "1.0",
            Url = $"{baseUrl}/a2a/pizza"
        };
    }

    public static AIAgent CreatePizzaAgent(string endpoint, string apiKey, string deploymentName, IServiceProvider sp)
    {
        var chatClient = new AzureOpenAIClient(
                new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .Use(
                getResponseFunc: (messages, options, innerChatClient, cancellationToken) =>
                    CustomChatClientMiddleware(messages, options, innerChatClient, sp, cancellationToken),
                getStreamingResponseFunc: null)
            .Build(sp);

        var agent = new ChatClientAgent(
                chatClient,
                instructions: "You are a pizza ordering agent. Speak like a stereotypical italian pizza chef." +
                              " Always start with 'Mama mia! '" +
                              "When asked for a pizza, call the 'Order Pizza' tool.",
                name: PizzaAgentName,
                description: "An agent that manage pizza ordering",
                tools: [AIFunctionFactory.Create(OrderPizzaTool.OrderPizza)],
                services: sp)
            .AsBuilder()
            .Use((agent, ctx, func, ct) => CustomFunctionCallingMiddleware(agent, ctx, func, sp, ct))
            .Use(runFunc: (msg, sess, opts, innerAgent, ct) =>
                    CustomAgentRunMiddleware(msg, sess, opts, innerAgent, sp, ct), runStreamingFunc: null)
            .Build(sp);

        return agent;
    }

    /* IChatClient middleware */
    private static async Task<ChatResponse> CustomChatClientMiddleware(
        IEnumerable<ChatMessage> messages,
        ChatOptions? options,
        IChatClient innerChatClient,
        IServiceProvider sp,
        CancellationToken cancellationToken)
    {
        var logger = sp.GetRequiredService<ILogger<PizzaAgentFactory>>();

        logger.LogInformation($"\tIChatClient middleware before");
        var response = await innerChatClient.GetResponseAsync(messages, options, cancellationToken);
        logger.LogInformation($"\tIChatClient middleware after : about to be deliver : {response.Messages.Last()}");

        return response;
    }


    /* Function calling middleware */
    private static async ValueTask<object?> CustomFunctionCallingMiddleware(
        AIAgent agent,
        FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
        IServiceProvider sp,
        CancellationToken cancellationToken)
    {
        var logger = sp.GetRequiredService<ILogger<PizzaAgentFactory>>();
        logger.LogInformation($"\t\tFunction calling middleware before - Function Name: {context!.Function.Name}");
        var result = await next(context, cancellationToken);
        logger.LogInformation($"\t\tFunction calling middleware after - Function Call Result: {result}");

        return result;
    }

    /* Agent Run Middleware */
    private static async Task<AgentResponse> CustomAgentRunMiddleware(
        IEnumerable<ChatMessage> messages,
        AgentSession? session,
        AgentRunOptions? options,
        AIAgent innerAgent,
        IServiceProvider sp,
        CancellationToken cancellationToken)
    {
        var logger = sp.GetRequiredService<ILogger<PizzaAgentFactory>>();
        logger.LogInformation($"Agent Run Middleware before");
        var response = await innerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);
        logger.LogInformation($"Agent Run Middleware after\n");
        return response;
    }
}