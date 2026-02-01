#pragma warning disable MEAI001

using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkQuickStart.Features;

public static class HumanInTheLoop
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Human In The Loop Function Call ---");

        var weatherFunction = AIFunctionFactory.Create(GeneralTools.GetWeather);
        var approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();
            
        var agent = new ChatClientAgent(
                chatClient: client,
                name: "UtilityToolAgent",
                instructions:
                "You are a utility assistant that can get the current date/time. When asked for this information, use your available tools.",
                description: "An agent that can get the current date/time.",
                tools: [approvalRequiredWeatherFunction, AIFunctionFactory.Create(GeneralTools.GetDateTime)]
            );
        
        AgentSession session = await agent.GetNewSessionAsync();
        var response = await agent.RunAsync("What is the weather like in Amsterdam?", session);
        var functionApprovalRequests = response.Messages
            .SelectMany(x => x.Contents)
            .OfType<FunctionApprovalRequestContent>()
            .ToList();

        if (functionApprovalRequests.Count > 0)
        {
            Console.WriteLine("--- Function Approval Requests ---");
            FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
            Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
            var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
            Console.WriteLine(await agent.RunAsync(approvalMessage, session));
        }

        Console.WriteLine();
    }
}