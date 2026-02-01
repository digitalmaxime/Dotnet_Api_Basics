using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;
using OpenAI.Chat;

namespace AgentFrameworkQuickStart.Features;

public static class AgentAsFunctionTool
{
    
    public static async Task Call(string endpoint, string deploymentName, string apiKey, AIAgent toolAgent)
    {
        Console.WriteLine("--- Agent as Function Tool ---");
        
        AIAgent mainAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .CreateAIAgent(
                name: "MainAgent",
                instructions: "You are a helpful assistant who responds in French.",
                tools: [toolAgent.AsAIFunction()]
            );

        var response = await mainAgent.RunAsync("Tell me the current date and time.");
        Console.WriteLine(response);
        Console.WriteLine();

    }

}