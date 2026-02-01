using System.Text.Json;
using AgentFrameworkQuickStart.Features.RAG.Models;
using AgentFrameworkQuickStart.Features.RAG.Schemas;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.PgVector;

namespace AgentFrameworkQuickStart.Features.RAG.Data;

public class RAGPersistence
{
    public static async Task<PostgresCollection<string, FoodVectorSchema>> SeedDataAsync(
        IEmbeddingGenerator embeddingGenerator, IConfiguration configuration)
    {
        var jsonData = await File.ReadAllTextAsync("./Features/RAG/Data/data.json");
        var food = JsonSerializer.Deserialize<Food[]>(jsonData, JsonSerializerOptions.Web)!;
        
        var connectionString = configuration.GetRequiredSection("ConnectionStrings:DefaultConnection").Get<string>();
        var vectorStore = new PostgresVectorStore(connectionString!, options: new PostgresVectorStoreOptions
        {
            EmbeddingGenerator = embeddingGenerator
        });
        
        var collection = vectorStore.GetCollection<string, FoodVectorSchema>("FoodVectorSchema");
        await collection.EnsureCollectionExistsAsync();

        var counter = 0;
        foreach (var item in food)
        {
            counter++;
            Console.Write($"\rEmbedding food: {counter}/{food.Length}");
            await collection.UpsertAsync(new FoodVectorSchema()
            {
                Id = Guid.NewGuid(),
                Title = item.Title,
                Content = item.Content,
                Price = item.Price
            });
        }

        return collection;
    }
}