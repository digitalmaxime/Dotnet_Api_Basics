using System;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

// var agent = new AzureOpenAIClient(
//     new Uri("https://<myresource>.openai.azure.com"),
//     new AzureCliCredential())
//         .GetChatClient(deployment)
//         .CreateAIAgent(instructions: "You are good at telling jokes.", name: "Joker");
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();

// 2. Get configuration values
var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;


AzureOpenAIClient azureClient = new(
    endpoint,
    new AzureKeyCredential(apiKey));
OpenAI.Chat.ChatClient chatClient = azureClient.GetChatClient(deploymentName);


List<ChatMessage> messages = new List<ChatMessage>()
{
    new SystemChatMessage("You are a helpful assistant."),
    new UserChatMessage("I am going to Paris, what should I see?"),
};

var response = chatClient.CompleteChat(messages);
Console.WriteLine(response.Value.Content[0].Text);
// Append the model response to the chat history.
messages.Add(new AssistantChatMessage(response.Value.Content[0].Text));
// Append new user question.
messages.Add(new UserChatMessage("What is so great about #1?"));

response = chatClient.CompleteChat(messages);
Console.WriteLine(response.Value.Content[0].Text);