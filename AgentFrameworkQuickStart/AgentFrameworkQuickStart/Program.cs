using System;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;


// Get configuration values
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();
var endpoint = configuration["AzureOpenAI:Endpoint"]!;
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

AIAgent agent = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey))
        .GetChatClient(deploymentName)
        .CreateAIAgent()
    // .CreateAIAgent(instructions: "You are good at telling jokes.");
    ;
Console.WriteLine(await agent.RunAsync("Tell me a joke about a pirate."));