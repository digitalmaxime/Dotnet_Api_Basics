using AgentFrameworkQuickStart.Features.RAG.Schemas;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.PgVector;

namespace AgentFrameworkQuickStart.Features.RAG.Tools;

public class RAGSearchTool(PostgresCollection<string, FoodVectorSchema> collection)
{
    public AITool SearchTool => AIFunctionFactory.Create(
        (string question) => SearchVectorStore(question),
        name: "SearchGroceryItems",
        description: "Finds food items in a grocery store catalog based on a user's question or search query."
    );


    public async Task<List<string>> SearchVectorStore(string question)
    {
        List<string> result = [];
        await foreach (var searchResult in collection.SearchAsync(question, 10,
                           new VectorSearchOptions<FoodVectorSchema>
                           {
                               IncludeVectors = false
                           }))
        {
            FoodVectorSchema record = searchResult.Record;
            result.Add(record.Title);
        }

        return result;
    }
}