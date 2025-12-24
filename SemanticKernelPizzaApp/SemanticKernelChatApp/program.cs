using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelWeatherApp;
using Microsoft.Extensions.DependencyInjection;
using SemanticKernelWeatherApp.ChatHistory;
using SemanticKernelWeatherApp.Plugins;

var builder = Kernel.CreateBuilder();
IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.private.json").Build();

// Get configuration values
var endpoint = new Uri(configuration["AzureOpenAI:Endpoint"]!);
var deploymentName = configuration["AzureOpenAI:DeploymentName"]!;
var apiKey = configuration["AzureOpenAI:ApiKey"]!;

// Configure the Kernel builder with Azure OpenAI
builder.AddAzureOpenAIChatCompletion(
    deploymentName,
    endpoint.ToString(),
    apiKey
);

// Add HttpClient service - this is required for an AI connector
builder.Services.AddHttpClient();

// Plugins for function calls
builder.Plugins.AddFromType<ArchivePlugin>();
builder.Plugins.AddFromType<PizzaPlugin>();

var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

var chatHistory = ChatHistoryHelper.InitializeChatHistory().SimulateFunctionCalls();

PromptExecutionSettings settings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.None(),
};
var toto = await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new (settings));
Console.WriteLine(toto);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("Ask > ");
    Console.Out.Flush();
    Console.ResetColor();
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) break;

    chatHistory.AddUserMessage(userInput);

    var response = chatService.GetStreamingChatMessageContentsAsync(
        chatHistory,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        },
        kernel: kernel
    );
    
    var fullAnswerBuilder = new StringBuilder();
    
    try
    {
        await foreach (var chunk in response)
        {
            if (chunk.Content is { Length: > 0 })
            {
                fullAnswerBuilder.Append(chunk.Content);
                Console.Write(chunk.Content);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError during streaming: {ex.Message}");
        return;
    }

    Console.WriteLine();

    chatHistory.AddAssistantMessage(fullAnswerBuilder.ToString());
    chatHistory = await chatHistory.ReduceChatHistoryAsync(chatService);
    
}
