using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelWeatherApp.ChatHistory;

public static class ChatHistoryHelper
{
    public static Microsoft.SemanticKernel.ChatCompletion.ChatHistory InitializeChatHistory()
    {
        Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory = [];
        chatHistory.AddSystemMessage(
            "You are mario the plumber! Assume the user is interested in ordering some pizza." +
            "Keep your responses short and concise. Always greet the user with a friendly 'It's-a me, Mario!'"
        );

        return chatHistory;
    }

    public static Microsoft.SemanticKernel.ChatCompletion.ChatHistory SimulateFunctionCalls(
        this Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
    {
        // Add a simulated function call from the assistant
        chatHistory.Add(
            new ChatMessageContent
            {
                Role = AuthorRole.Assistant,
                Items =
                [
                    new FunctionCallContent(
                        functionName: "get_user_allergies",
                        pluginName: "User",
                        id: "0001",
                        arguments: new KernelArguments { { "username", "laimonisdumins" } }
                    ),
                    new FunctionCallContent(
                        functionName: "get_user_allergies",
                        pluginName: "User",
                        id: "0002",
                        arguments: new KernelArguments { { "username", "emavargova" } }
                    )
                ]
            }
        );

        // Add a simulated function results from the tool role
        chatHistory.Add(
            new ChatMessageContent
            {
                Role = AuthorRole.Tool,
                Items =
                [
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
            new ChatMessageContent
            {
                Role = AuthorRole.Tool,
                Items =
                [
                    new FunctionResultContent(
                        functionName: "get_user_allergies",
                        pluginName: "User",
                        callId: "0002",
                        result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
                    )
                ]
            }
        );

        return chatHistory;
    }

    public static async Task<Microsoft.SemanticKernel.ChatCompletion.ChatHistory> ReduceChatHistoryAsync(this Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory,
        IChatCompletionService chatService)
    {
        ArgumentNullException.ThrowIfNull(chatHistory);
        ArgumentNullException.ThrowIfNull(chatService);
        var summarizationReducer = new ChatHistorySummarizationReducer(chatService, targetCount: 3, thresholdCount: 4);
        var reducedMessages = await summarizationReducer.ReduceAsync(chatHistory);
        chatHistory = reducedMessages != null 
            ? new Microsoft.SemanticKernel.ChatCompletion.ChatHistory(reducedMessages)
            : chatHistory;
        
        Console.WriteLine($"Chat history contains {chatHistory.Count} messages");
        return chatHistory;
    }
}