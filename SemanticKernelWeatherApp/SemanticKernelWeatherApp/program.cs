using Codeblaze.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using SemanticKernelWeatherApp;
using Microsoft.Extensions.DependencyInjection;

var builder = Kernel.CreateBuilder();

// Services
// builder.AddAzureOpenAIChatCompletion("model-name", "deployment-name", "api-key", "endpoint");
// builder.AddOpenAIChatCompletion("model-name", "deployment-name", "api-key", "endpoint");
// Add HttpClient service - this is required for Ollama connector
builder.Services.AddHttpClient();

builder.AddOllamaChatCompletion(
    modelId: "llama3",
    baseUrl: new Uri("http://localhost:11434")
);


// Plugins
builder.Plugins.AddFromType<NewsFeedPlugin>();
builder.Plugins.AddFromType<ArchivePlugin>();


var kernel = builder.Build();

var chatService = kernel.GetRequiredService<IChatCompletionService>();

// Persona
ChatHistory chatHistory =
    new ChatHistory(
        "You are mario the plumber! If the user doesn't provider a news catagory, assume they want technology news and mention it to them. Keep your responses short and concise. Always greet the user with a friendly 'It's-a me, Mario!'");


while (true)
{
    Console.WriteLine(">");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput)) break;
    chatHistory.AddUserMessage(userInput);
    var response = chatService.GetStreamingChatMessageContentsAsync(
        chatHistory,
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        },
        kernel: kernel
    );
    string fullAnswer = "";
    await foreach (var chunk in response)
    {
        if (chunk.Content.Length > 0)
        {
            fullAnswer += chunk.Content;
            Console.Write(chunk);
        }
    }

    Console.WriteLine(fullAnswer);
    chatHistory.AddAssistantMessage(fullAnswer);
    Console.WriteLine("\n\n\n");
}