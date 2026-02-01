using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkQuickStart.Features;

public class BasicAgent
{
    public static async Task<AIAgent> Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Basic Agent ---");

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();

        var agent = new ChatClientAgent(
            chatClient: client,
            name: "UtilityToolAgent",
            instructions:
            "You are a utility assistant that can get the current date/time. When asked for this information, use your available tools.",
            description: "An agent that can get the current date/time.",
            tools: [AIFunctionFactory.Create(GeneralTools.GetDateTime)]
        );

        Console.WriteLine(await agent.RunAsync("Tell me a Mitch Hedberg joke"));
        Console.WriteLine();

        return agent;
    }
}