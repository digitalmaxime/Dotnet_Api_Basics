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

// 3. Configure the Kernel builder with Azure OpenAI
builder.AddAzureOpenAIChatCompletion(
    deploymentName,
    endpoint.ToString(),
    apiKey
);

builder.Services.AddHttpClient();


// Plugins
builder.Plugins.AddFromType<NewsFeedPlugin>();
builder.Plugins.AddFromType<ArchivePlugin>();


var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Persona
ChatHistory chatHistory = [];
chatHistory.AddSystemMessage(
    "You are mario the plumber! Assume the user is interested in ordering some pizza." +
    "Keep your responses short and concise. Always greet the user with a friendly 'It's-a me, Mario!'"
    );

// Add a simulated function call from the assistant
chatHistory.Add(
    new()
    {
        Role = AuthorRole.Assistant,
        Items = [
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0001",
                arguments: new () { {"username", "laimonisdumins"} }
            ),
            new FunctionCallContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                id: "0002",
                arguments: new () { {"username", "emavargova"} }
            )
        ]
    }
);

// Add a simulated function results from the tool role
chatHistory.Add(
    new()
    {
        Role = AuthorRole.Tool,
        Items = [
            new FunctionResultContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                callId: "0001",
                result: "{ \"allergies\": [\"peanuts\", \"gluten\"] }"
            )
        ]
    }
);
chatHistory.Add(
    new()
    {
        Role = AuthorRole.Tool,
        Items = [
            new FunctionResultContent(
                functionName: "get_user_allergies",
                pluginName: "User",
                callId: "0002",
                result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
            )
        ]
    }
);

PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.None() };
var toto = await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new (settings));
System.Console.WriteLine(toto);
// extra commit 1
// extra commit 3
while (true)
{
    Console.WriteLine("Ask >");

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

Console.WriteLine("--------------------------");

// Get the current length of the chat history object
int currentChatHistoryLength = chatHistory.Count;

// Get the chat message content
ChatMessageContent results = await chatService.GetChatMessageContentAsync(
    chatHistory,
    kernel: kernel
);

// Get the new messages added to the chat history object
for (int i = 0; i < chatHistory.Count; i++)
{
    Console.WriteLine(chatHistory[i]);
}

// Print the final message
Console.WriteLine(results);

// Add the final message to the chat history object
chatHistory.Add(results);