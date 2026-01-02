using System.Text.Json;
using AgentFrameworkQuickStart.models;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;

namespace AgentFrameworkQuickStart.features;

public static class StructuredOutput
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey)
    {
        Console.WriteLine("--- Structured Output ---");
        
        JsonElement schema = AIJsonUtilities.CreateJsonSchema(typeof(PersonInfo));
        ChatOptions chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(
                schema: schema,
                schemaName: nameof(PersonInfo),
                schemaDescription: "Information about a person including their name, age, and occupation")
        };
        AIAgent structuredOutputAgent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .CreateAIAgent(new ChatClientAgentOptions()
            {
                Name = "HelpfulAssistant",
                ChatOptions = chatOptions
            });
        var response =
            await structuredOutputAgent.RunAsync(
                "Please provide information about John Smith, who is a 35-year-old software engineer.");
        var personInfo = response.Deserialize<PersonInfo>(JsonSerializerOptions.Web);
        Console.WriteLine($"Name: {personInfo.Name}, Age: {personInfo.Age}, Occupation: {personInfo.Occupation}");
        Console.WriteLine();

    }
}