using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;

namespace AgentFrameworkQuickStart.Features;

public static class AgentAsFunctionTool
{
    
    public static async Task Call(string endpoint, string deploymentName, string apiKey, AIAgent toolAgent)
    {
        Console.WriteLine("--- Agent as Function Tool ---");
        
        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();
            
            var agent = new ChatClientAgent(
                chatClient: client,
                name: "MainAgent",
                instructions: "You are a helpful assistant who responds in French.",
                tools: [toolAgent.AsAIFunction()]
            );

        var response = await agent.RunAsync("Tell me the current date and time.");
        Console.WriteLine(response);
        Console.WriteLine();

    }

}