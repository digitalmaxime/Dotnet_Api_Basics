



# Semantic Kernel 

Quick start guide from Microsoft using gpt-4 hosted in AzureFoundry (personal account)

[semantic kernel microsoft doc](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/)

## Chat History

Chat history object `ChatHistory chatHistory = []`

System, User and Assistant messages are available

```csharp
chatHistory.AddSystemMessage("You are a helpful assistant.");
chatHistory.AddUserMessage("What's available to order?");
chatHistory.AddAssistantMessage("We have everything");
```

#### Simulating function calls

```csharp
chatHistory.Add(
    new() {
        Role = AuthorRole.Assistant,
        Items = [
            new FunctionCallContent(
                ...

chatHistory.Add(
    new() {
        Role = AuthorRole.Tool,
        Items = [
            new FunctionResultContent(
                ...
```

#### Inspecting a chat history object

```csharp
ChatMessageContent results = await chatCompletionService.GetChatMessageContentAsync(
    chatHistory,
    kernel: kernel
);
```

#### Chat History Reduction

Strategies 
- Truncation
- Summarization
- Token-Based

Can be achieved by implementing this interface

```csharp
namespace Microsoft.SemanticKernel.ChatCompletion;

[Experimental("SKEXP0001")]
public interface IChatHistoryReducer
{
    Task<IEnumerable<ChatMessageContent>?> ReduceAsync(IReadOnlyList<ChatMessageContent> chatHistory, CancellationToken cancellationToken = default);
}
```

or using built-in methods

```csharp
var reducer = new ChatHistoryTruncationReducer(targetCount: 2); // Keep system message and last user message
var reducedMessages = await reducer.ReduceAsync(chatHistory);

if (reducedMessages is not null)
{
    chatHistory = new ChatHistory(reducedMessages);
}
```
See [chat history microsoft doc](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/chat-history?pivots=programming-language-csharp)

## Function calling with chat completion

Auto function calling is the default behavior in Semantic Kernel, but you can also manually invoke functions if you prefer. For more information on manual function invocation, please refer to the [function invocation article](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/function-invocation#manual-function-invocation).

Once the functions are serialized (using JSON schema), they are sent to the model along with the current chat history. This allows the model to understand the context of the conversation and the available functions. The model processes the input and generates a response. The response can either be a chat message or one or more function calls. 

If the response is a chat message, it is returned to the caller. If the response is a function call, however, Semantic Kernel extracts the function name and its parameters.

The extracted function name and parameters are used to invoke the function in the kernel. 
The result of the function is then sent back to the model as part of the chat history.

You add functions as registered 'plugins'

```csharp
kernelBuilder.Plugins.AddFromType<OrderPizzaPlugin>("OrderPizza");
```

```csharp
public class OrderPizzaPlugin(IPizzaService pizzaService)
{
    [KernelFunction("get_pizza_menu")]
    public async Task<Menu> GetPizzaMenuAsync()
    {
        return await pizzaService.GetMenu();
    }

    [KernelFunction("add_pizza_to_cart")]
    [Description("Add a pizza to the user's cart; returns the new item and updated cart")]
    public async Task<CartDelta> AddPizzaToCart(PizzaSize size, int quantity = 1)
    {
        ...
```

** Note : Only functions with the `KernelFunction` attribute will be serialized and sent to the model. This allows you to have helper functions that are not exposed to the model.

When you create a kernel with the OrderPizzaPlugin, the kernel will automatically serialize the functions and their parameters. This is necessary so that the model can understand the functions and their inputs.

Tip 💡
Avoid, where possible, using string as a parameter type. The model can't infer the type of string, which can lead to ambiguous responses. Instead, use enums or other types (e.g., int, float, and complex types) where possible.

Tip 💡
Before adding a description, ask yourself if the model needs this information to generate a response. If not, consider leaving it out to reduce verbosity. You can always add descriptions later if the model is struggling to use the function properly. 

Tip 💡
To ensure a model can self-correct, it's important to provide error messages that clearly communicate what went wrong and how to fix it. This can help the model retry the function call with the correct information.

See [function calling microsoft doc](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/?pivots=programming-language-csharp)

## Function Choice Behaviors

Function choice behaviors are bits of configuration that allows a developer to configure:
- Which functions are advertised to AI models.
- How the models should choose them for invocation.
- How Semantic Kernel might invoke those functions.

As of today, the function choice behaviors are represented by three static methods of the FunctionChoiceBehavior class:

- `FunctionChoiceBehavior.Auto`: Allows the AI model to choose from zero or more function(s) from the provided function(s) for invocation.
- `FunctionChoiceBehavior.Required`: Forces the AI model to choose one or more function(s) from the provided function(s) for invocation.
- `FunctionChoiceBehavior.None`: Instructs the AI model not to choose any function(s).

All three function choice behaviors accept a list of functions to advertise as a functions parameter. By default, it is null, which means all functions from plugins registered on the Kernel are provided to the AI model.
You can decide which functions are exposed / advertised to the AI model by passing a list of functions/plugins to the `FunctionChoiceBehavior` config.


```csharp
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(functions: [getWeatherForCity, getCurrentTime]) }; 
await kernel.InvokePromptAsync("Given the current time of day and weather, what is the likely color of the sky in Boston?", new(settings));
```

 See [Function Advertising](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/function-choice-behaviors?pivots=programming-language-csharp#function-advertising)

## Function Invocation Modes

When the AI model receives a prompt containing a list of functions, it may choose one or more of them for invocation to complete the prompt. When a function is chosen by the model, it needs be invoked by Semantic Kernel.

The function calling subsystem in Semantic Kernel has two modes of function invocation: auto and manual.


#### Auto Function Invocation

Auto function invocation is the default mode of the Semantic Kernel function-calling subsystem. When the AI model chooses one or more functions, Semantic Kernel **automatically invokes** the chosen functions. The results of these function invocations are added to the chat history and sent to the model automatically in subsequent requests. The model then reasons about the chat history, chooses additional functions if needed, or generates the final response. This approach is fully automated and requires no manual intervention from the caller.

Tip 💡
Auto `function invocation` dictates if functions should be ***invoked*** automatically by Semantic Kernel. While Auto function `choice behavior` determines if functions should be ***chosen*** automatically by the AI model.

#### Manual Function Invocation

In cases when the caller wants to have more control over the function invocation process, manual function invocation can be used.

When manual function invocation is enabled, Semantic Kernel does not automatically invoke the functions chosen by the AI model. Instead, Semantic Kernel returns a list of chosen functions to the caller, who can then decide which functions to invoke, invoke them sequentially or in parallel, handle exceptions, and so on. The function invocation results need to be added to the chat history and returned to the model, which will reason about them and decide whether to choose additional functions or generate a final response.

```csharp
// Manual function invocation needs to be enabled explicitly by setting autoInvoke to false.
PromptExecutionSettings settings = new() { FunctionChoiceBehavior = Microsoft.SemanticKernel.FunctionChoiceBehavior.Auto(autoInvoke: false) };
```

See [Auto Function Invocation](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/function-calling/function-invocation?pivots=programming-language-csharp#auto-function-invocation)

## Using Ollama with Docker

`docker pull ollama/ollama`

`docker run -d -v ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama`

`docker exec -it ollama ollama pull llama3`

package : `<PackageReference Include="Codeblaze.SemanticKernel.Connectors.Ollama" Version="1.3.1" />`

```csharp
using Codeblaze.SemanticKernel.Connectors.Ollama;

...

builder.AddOllamaChatCompletion(
    modelId: "llama3",
    baseUrl: new Uri("http://localhost:11434")
);
```

## Reference

See [learn.microsoft.com/en-us/semantic-kernel](https://learn.microsoft.com/en-us/semantic-kernel/concepts/ai-services/chat-completion/?tabs=csharp-AzureOpenAI%2Cpython-AzureOpenAI%2Cjava-AzureOpenAI&pivots=programming-language-csharp)