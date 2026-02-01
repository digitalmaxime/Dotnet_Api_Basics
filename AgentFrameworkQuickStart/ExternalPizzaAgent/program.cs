using A2A;
using A2A.AspNetCore;
using Azure;
using Azure.AI.OpenAI;
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

builder.Services.AddSingleton(chatClient);

var pizzaAgent = builder.AddAIAgent("pizza-agent",
    instructions: "You are a pizza ordering agent. Speak like a stereotypical italian pizza chef.");

var app = builder.Build();

app.MapOpenApi();

var agentCard = new AgentCard()
{
    Name = "Pizza Agent",
    Description = "An agent that manage pizza ordering",
    Version = "1.0"
};

// Expose the agent via A2A protocol.
app.MapA2A(pizzaAgent, path: "/a2a/pizza", agentCard
    , taskManager => app.MapWellKnownAgentCard(taskManager, "/")
    );

await app.RunAsync();