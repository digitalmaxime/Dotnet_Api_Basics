#pragma warning disable MEAI001
using System.Text.Json;
using AgentFrameworkQuickStart.models;
using AgentFrameworkQuickStart.tools;
using Azure;
using Azure.AI.OpenAI;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;


// Get configuration values
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

var weatherFunction = AIFunctionFactory.Create(Tools.GetWeather);
var approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .CreateAIAgent(
        name: "UtilityToolAgent",
        instructions:
        "You are a utility assistant that can get the current date/time. When asked for this information, use your available tools.",
        description: "An agent that can get the current date/time.",
        tools: [approvalRequiredWeatherFunction, AIFunctionFactory.Create(Tools.GetDateTime)]
    );

// await BasicAgent(agent);
//
// await HumanInTheLoopFunctionCall(agent);
//
// await StructuredOutput(endpoint, deploymentName, apiKey);
//
// await AgentAsFunctionTool(endpoint, deploymentName, apiKey, agent);

await UseTelemetry(endpoint, deploymentName, apiKey, agent);

return;

#region Basic Agent

static async Task BasicAgent(AIAgent agent)
{
    Console.WriteLine("--- Basic Agent ---");
    Console.WriteLine(await agent.RunAsync("Tell me a joke."));
}

#endregion

#region Human In The Loop Function Call

static async Task HumanInTheLoopFunctionCall(AIAgent agent)
{
    Console.WriteLine("--- Human In The Loop Function Call ---");
    AgentThread thread = agent.GetNewThread();
    AgentRunResponse response = await agent.RunAsync("What is the weather like in Amsterdam?", thread);
    var functionApprovalRequests = response.Messages
        .SelectMany(x => x.Contents)
        .OfType<FunctionApprovalRequestContent>()
        .ToList();

    FunctionApprovalRequestContent requestContent = functionApprovalRequests.First();
    Console.WriteLine($"We require approval to execute '{requestContent.FunctionCall.Name}'");
    var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
    Console.WriteLine(await agent.RunAsync(approvalMessage, thread));
}

#endregion

#region Structured Output

static async Task StructuredOutput(string endpoint, string deploymentName, string apiKey)
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
}

#endregion

#region AI Function Tool

static async Task AgentAsFunctionTool(string endpoint, string deploymentName, string apiKey, AIAgent toolAgent)
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
}

#endregion

#region Telemetry

static async Task UseTelemetry(string endpoint, string deploymentName, string apiKey, AIAgent toolAgent)
{
    Console.WriteLine("Make sure Jeeger container is started for trace visualization");
    
    const string agentTelemetrySource = "agent-telemetry-source";
    
    // Create a TracerProvider that exports to the OtlpExporter (open telemetry protocol)
    using var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DemoAgent"))
        .AddSource(agentTelemetrySource)
        .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://localhost:4317"))
        .Build();
    
    var azureClient = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
    var chatClient = azureClient
        .GetChatClient(deploymentName)
        .CreateAIAgent(instructions: "You are good at telling jokes. about telemetry", name: "Joker")
        .AsBuilder()
        .UseOpenTelemetry(sourceName: agentTelemetrySource, configure: c => c.EnableSensitiveData = true)
        .Build();
 
    Console.WriteLine(await chatClient.RunAsync("Tell me a space exploration joke."));
}

#endregion
