using A2A;
using A2A.AspNetCore;
using Azure;
using Azure.AI.OpenAI;
using ExternalPizzaAgent.Tools;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder();

builder.Configuration.AddUserSecrets<Program>(optional: true);

var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = builder.Configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]!;

Console.WriteLine("--- A2A External Agent (Pizza) Starting on port 5001 ---");

builder.WebHost.UseUrls("http://localhost:5001");

builder.Services.AddOpenApi();

// Register the chat client
IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint), new AzureKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .AsIChatClient();

var agent = new ChatClientAgent(
    chatClient, 
    instructions: "You are a pizza ordering agent. Speak like a stereotypical italian pizza chef." +
                  " Always start with 'Mama mia! '" +
                  "When asked for a pizza, call the 'Order Pizza' tool.",
    name: "Pizza Agent",
    description: "An agent that manage pizza ordering",
    tools: [AIFunctionFactory.Create(OrderPizzaTool.OrderPizza)]);

builder.Services.AddSingleton(chatClient);

var app = builder.Build();

app.MapOpenApi();

var agentCard = new AgentCard()
{
    Name = "Pizza Agent",
    Description = "An agent that manage pizza ordering",
    Version = "1.0",
    Url = "http://localhost:5001/a2a/pizza"
};

// Expose the agent via A2A protocol.
app.MapA2A(agent, path: "/a2a/pizza", agentCard, taskManager => app.MapWellKnownAgentCard(taskManager, "/"));

await app.RunAsync();