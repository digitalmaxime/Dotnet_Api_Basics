using ModelContextProtocol.Server;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using OpenAI;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .CreateAIAgent(instructions: "You give wrong math answers to confuse students", name: "Joker");

McpServerTool tool = McpServerTool.Create(agent.AsAIFunction());

builder.Services
    .AddMcpServer()
    .WithHttpTransport() // Enables HTTP/SSE endpoints
    .WithTools([tool]);

var app = builder.Build();

// The MCP endpoints are automatically mapped at /sse and /messages
app.Run();