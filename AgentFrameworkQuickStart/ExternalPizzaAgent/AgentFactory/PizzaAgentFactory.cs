using A2A;
using Azure;
using Azure.AI.OpenAI;
using ExternalPizzaAgent.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace ExternalPizzaAgent.AgentFactory;

public static class PizzaAgentFactory
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

    public static AIAgent CreatePizzaAgent(string endpoint, string apiKey, string deploymentName)
    {
        var chatClient = new AzureOpenAIClient(
                new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();

        var agent = new ChatClientAgent(
            chatClient,
            instructions: "You are a pizza ordering agent. Speak like a stereotypical italian pizza chef." +
                          " Always start with 'Mama mia! '" +
                          "When asked for a pizza, call the 'Order Pizza' tool.",
            name:  PizzaAgentName,
            description: "An agent that manage pizza ordering",
            tools: [AIFunctionFactory.Create(OrderPizzaTool.OrderPizza)]);

        return agent;
    }
}