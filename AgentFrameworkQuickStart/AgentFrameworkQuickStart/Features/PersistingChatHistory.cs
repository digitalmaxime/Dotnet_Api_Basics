using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;
using OpenAI;

namespace AgentFrameworkQuickStart.Features;

public static class PersistingChatHistory
{
    /*
     * Using Postgres as a vector store to persist chat history
     * Make sure you have a Postgres database running locally.
     */
    public static async Task Call(string endpoint, string deploymentName, string apiKey,
        IConfiguration configuration)
    {
        Console.WriteLine("--- Persisting Chat History ---");
        
        var connectionString = configuration.GetRequiredSection("ConnectionStrings:DefaultConnection").Get<string>();
        var vectorStore = new PostgresVectorStore(connectionString!);

        AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey))
            .GetChatClient(deploymentName)
            .CreateAIAgent(new ChatClientAgentOptions
            {
                Name = "MyOptions",
                Description = "Options for persisting chat history in a vector store.",
                ChatOptions = new ChatOptions()
                {
                    Instructions = "Respond with Mark Normand joke.",
                },
                // 💡 Create a new chat message store for this agent that stores the messages in a vector store.
                ChatMessageStoreFactory = ctx => new MyChatMessageStore(
                    vectorStore,
                    ctx.SerializedState,
                    ctx.JsonSerializerOptions)
            });

        AgentThread thread = agent.GetNewThread();
        Console.WriteLine(await agent.RunAsync("Tell me a joke.", thread));
        Console.WriteLine(await agent.RunAsync("Tell me that same joke but in french.", thread));
        Console.WriteLine();

    }
}

internal class MyChatMessageStore : ChatMessageStore
{
    private readonly VectorStore _vectorStore;
    private string? ThreadDbKey { get; set; }

    public MyChatMessageStore(VectorStore vectorStore,
        JsonElement serializedStoreState,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));
        if (serializedStoreState.ValueKind is JsonValueKind.String)
        {
            ThreadDbKey = serializedStoreState.Deserialize<string>();
        }
    }

    public override async Task<IEnumerable<ChatMessage>> GetMessagesAsync(CancellationToken cancellationToken = default)
    {
        var collection = _vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        var records = collection
            .GetAsync(
                x => x.ThreadId == this.ThreadDbKey, top: 10,
                new FilteredRecordRetrievalOptions<ChatHistoryItem>
                    { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken);

        List<ChatMessage> messages = [];
        await foreach (var record in records)
        {
            messages.Add(JsonSerializer.Deserialize<ChatMessage>(record.SerializedMessage!)!);
        }

        messages.Reverse();
        return messages;
    }

    public override async Task AddMessagesAsync(IEnumerable<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        // TODO: summarize messages 
        ThreadDbKey ??= Guid.NewGuid().ToString("N");
        var collection = _vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);
        await collection.UpsertAsync(messages.Select(x => new ChatHistoryItem
        {
            Key = ThreadDbKey + x.MessageId,
            Timestamp = DateTimeOffset.UtcNow,
            ThreadId = ThreadDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        // We have to serialize the thread id, so that on deserialization you can retrieve the messages using the same thread id.
        JsonSerializer.SerializeToElement(ThreadDbKey);

    private sealed class ChatHistoryItem
    {
        [VectorStoreKey] public string? Key { get; set; }
        [VectorStoreData] public string? ThreadId { get; set; }
        [VectorStoreData] public DateTimeOffset? Timestamp { get; set; }
        [VectorStoreData] public string? SerializedMessage { get; set; }
        [VectorStoreData] public string? MessageText { get; set; }
    }
}