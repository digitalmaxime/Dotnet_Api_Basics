using System.Text.Json;
using AgentFrameworkQuickStart.Models;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkQuickStart.Features;

public static class StructuredOutput
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Structured Output ---");

        JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(PersonInfo));

        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .AsIChatClient();
        
        ChatOptions chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                schema: schema,
                schemaName: nameof(PersonInfo),
                schemaDescription: "Information about a person including their name, age, and occupation")
        };

        var chatClientAgentOptions = new ChatClientAgentOptions
        {
            Name = "HelpfulAssistant",
            Description = "Chat agent that get information about people",
            ChatOptions = chatOptions
        };

        var agent = new ChatClientAgent(client, chatClientAgentOptions);

        var response =
            await agent.RunAsync("Please provide information about John Doe, who is a 35-year-old software engineer.");

        var personInfo = response.Deserialize<PersonInfo>(JsonSerializerOptions.Web);

        Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
        Console.WriteLine();
    }
}