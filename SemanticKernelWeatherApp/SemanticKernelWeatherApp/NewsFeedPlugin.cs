using System.ComponentModel;
using SimpleFeedReader;
using Microsoft.SemanticKernel;

namespace SemanticKernelWeatherApp;

public class NewsFeedPlugin 
{
    [KernelFunction("get_news")]
    [Description("Gets the latest news articles from a specified category.")]
    [return: Description("The latest news articles from a specified category.")]
    public async Task<List<FeedItem>> GetNews(Kernel kernel, string category)
    {
        try
        {
            // Create a feed reader instance
            var feedReader = new FeedReader();
            
            // Define RSS feed URLs based on category
            var feedUrl = GetFeedUrlByCategory(category);
            
            if (string.IsNullOrEmpty(feedUrl))
            {
                return new List<FeedItem>();
            }
            
            // Read the feed
            var feedItems = await feedReader.RetrieveFeedAsync(feedUrl);
            
            return feedItems?.Take(5).ToList() ?? new List<FeedItem>();
        }
        catch (Exception)
        {
            Console.WriteLine("Error fetching news feed.");
            return new List<FeedItem>();
        }
    }
    
    private string GetFeedUrlByCategory(string category)
    {
        return category?.ToLowerInvariant() switch
        {
            "technology" => "https://feeds.feedburner.com/oreilly/radar",
            "news" => "https://rss.cnn.com/rss/edition.rss",
            "science" => "https://www.sciencedaily.com/rss/top.xml",
            "business" => "https://feeds.feedburner.com/entrepreneur/latest",
            _ => string.Empty
        };
    }
}