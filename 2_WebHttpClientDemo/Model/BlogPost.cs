namespace _2_WebHttpClientDemo.Model;

public record BlogPost
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}