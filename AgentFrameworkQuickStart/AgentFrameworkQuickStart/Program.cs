using AgentFrameworkQuickStart.Features;
using AgentFrameworkQuickStart.Features.A2A;
using AgentFrameworkQuickStart.Features.RAG;
using AgentFrameworkQuickStart.Features.Workflows;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>() // Since you have UserSecretsId configured
    .AddEnvironmentVariables()
    .Build();

var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

// var agent = await BasicAgent.Call(endpoint, deploymentName, apiKey);

// await HumanInTheLoop.Call(endpoint, deploymentName, apiKey);

// await StructuredOutput.Call(endpoint, deploymentName, apiKey);

// await AgentAsFunctionTool.Call(endpoint, deploymentName, apiKey, agent);

// await Telemetry.Call(endpoint, deploymentName, apiKey);

// await PersistingChatHistory.Call(endpoint, deploymentName, apiKey, configuration);

// await SimpleSequentialWorkflow.Call();

// await ConcurrentWorkflow.Call(endpoint, deploymentName, apiKey); // WIP

// await RAGAgent.Call(endpoint, deploymentName, apiKey, configuration); // WIP

await MiddlewareAgent.Call(endpoint, deploymentName, apiKey);

// await A2AAgent.Call(endpoint, deploymentName, apiKey);