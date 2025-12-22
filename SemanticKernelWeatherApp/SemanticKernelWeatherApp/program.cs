using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelWeatherApp;
using Microsoft.Extensions.DependencyInjection;

var builder = Kernel.CreateBuilder();

// Add HttpClient service - this is required for Ollama connector
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();

// 2. Get configuration values
var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;
builder.AddAzureOpenAIChatCompletion(
    deploymentName,
    endpoint.ToString(),
    apiKey
);

builder.Services.AddHttpClient();

// builder.AddOllamaChatCompletion(
//     modelId: "llama3",
//     baseUrl: new Uri("http://localhost:11434")
// );


// Plugins
builder.Plugins.AddFromType<NewsFeedPlugin>();
builder.Plugins.AddFromType<ArchivePlugin>();


var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Persona
ChatHistory chatHistory =
    new ChatHistory(
        "You are mario the plumber! If the user doesn't provider a news catagory, " +
        "assume they want technology news and mention it to them. Keep your responses short and concise. " +
        "Always greet the user with a friendly 'It's-a me, Mario!'");


while (true)
{
    Console.WriteLine("enter question>");
    
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(userInput)) break;
    
    chatHistory.AddUserMessage(userInput);
    
    // var response0 = chatService.GetStreamingChatMessageContentsAsync(
    //     chatHistory: chatHistory,
    //     executionSettings: new OpenAIPromptExecutionSettings()
    //     {
    //         ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    //     },
    //     kernel: kernel
    // );
    //
    // await foreach (var chunk in response0)
    // {
    //     Console.Write(chunk);
    // }
    //
    
    
    
    
    
    var response = chatService.GetStreamingChatMessageContentsAsync(
        chatHistory,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel: kernel
    );
    var fullAnswer = "";
    await foreach (var chunk in response)
    {
        if (chunk.Content is { Length: > 0 })
        {
            fullAnswer += chunk.Content;
            Console.Write(chunk);
        }
    }
    
    Console.WriteLine(fullAnswer);
    chatHistory.AddAssistantMessage(fullAnswer);
    Console.WriteLine("\n\n");
}