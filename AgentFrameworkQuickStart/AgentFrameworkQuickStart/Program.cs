using AgentFrameworkQuickStart.features;
using AgentFrameworkQuickStart.tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;


IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

var agent = await BasicAgent(endpoint, deploymentName, apiKey);

await HumanInTheLoop.Call(endpoint, deploymentName, apiKey);

await StructuredOutput.Call(endpoint, deploymentName, apiKey);

await AgentAsFunctionTool.Call(endpoint, deploymentName, apiKey, agent);

await Telemetry.Call(endpoint, deploymentName, apiKey);

await PersistingChatHistory.Call(endpoint, deploymentName, apiKey, configuration);

return;

#region Basic Agent

static async Task<AIAgent> BasicAgent(string endpoint, string deploymentName, string apiKey)
{
    Console.WriteLine("--- Basic Agent ---");
    
    AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
        .GetChatClient(deploymentName)
        .CreateAIAgent(
            name: "UtilityToolAgent",
            instructions:
            "You are a utility assistant that can get the current date/time. When asked for this information, use your available tools.",
            description: "An agent that can get the current date/time.",
            tools: [AIFunctionFactory.Create(Tools.GetDateTime)]
        );
    
    Console.WriteLine(await agent.RunAsync("Tell me a what day it is in a surprising way"));
    Console.WriteLine();

    return agent;
}

#endregion
