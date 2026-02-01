using System.Text.Json;
using AgentFrameworkQuickStart.Features.RAG.Data;
using AgentFrameworkQuickStart.Features.RAG.Schemas;
using AgentFrameworkQuickStart.Features.RAG.Tools;
using AgentFrameworkQuickStart.Tools;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.Connectors.PgVector;
using OpenAI;

namespace AgentFrameworkQuickStart.Features.RAG;

public class RAGAgent
{
    public static async Task Call(string endpoint, string deploymentName, string apiKey, IConfiguration configuration)
    {
        Console.WriteLine("--- Basic Agent ---");
    
        
        var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            
        var embeddingGenerator = client
            .GetEmbeddingClient("text-embedding-3-small")
            .AsIEmbeddingGenerator();
            
        var collection = await RAGPersistence.SeedDataAsync(embeddingGenerator, configuration);
        
        RAGSearchTool searchTool = new(collection);
        
        var agent = client.GetChatClient(deploymentName).CreateAIAgent(
            name: "Grocery food item agent",
            instructions: "You are a grocery food item manager agent",
            tools: [searchTool.SearchTool]
            );
        
        var response = await agent.RunAsync("what products are closely related to coffee bean?");
        Console.WriteLine(response);
        
        Console.WriteLine();
        Console.WriteLine("\rEmbedding complete... Let's ask the question again using RAG");
        
        // var response = await agent.RunAsync("Tell me a what day it is in a surprising way");
        // Console.WriteLine(response);
        // Console.WriteLine(response.Usage?.TotalTokenCount.ToString());
        Console.WriteLine();

    }
}