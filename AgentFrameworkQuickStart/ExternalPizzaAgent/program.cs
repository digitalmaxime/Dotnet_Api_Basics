using A2A.AspNetCore;
using ExternalPizzaAgent.AgentFactory;
using Microsoft.Agents.AI.Hosting;


var builder = WebApplication.CreateBuilder();
const string baseUrl = "http://localhost:5001";
builder.WebHost.UseUrls(baseUrl);
builder.Configuration.AddUserSecrets<Program>(optional: true);

var endpoint = builder.Configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = builder.Configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = builder.Configuration["AzureOpenAI:ApiKey"]!;

var hostedAgentBuilder = builder.Services.AddAIAgent(PizzaAgentFactory.PizzaAgentName,
    (sp, name) => PizzaAgentFactory.CreatePizzaAgent(endpoint, apiKey, deploymentName));

var app = builder.Build();

// Expose the agent via A2A protocol.
app.MapA2A(hostedAgentBuilder, path: "/a2a/pizza", PizzaAgentFactory.CreateAgentCard(baseUrl),
    taskManager => app.MapWellKnownAgentCard(taskManager, "/"));

await app.RunAsync();