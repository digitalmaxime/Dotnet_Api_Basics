using Microsoft.Extensions.VectorData;

namespace AgentFrameworkQuickStart.Features.RAG.Schemas;

public class FoodVectorSchema
{
    

    [VectorStoreKey]
    public required Guid Id { get; set; }
    
    [VectorStoreData] 
    public required string Title { get; set; }
    
    
    [VectorStoreData] 
    public required string Content { get; set; }
    
    
    [VectorStoreData] 
    public required double Price { get; set; }

    [VectorStoreVector(1536)] 
    public string Vector => $"Title: {Title} - Content: {Content} - Price: {Price}";
}