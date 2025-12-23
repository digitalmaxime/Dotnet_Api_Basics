using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SemanticKernelWeatherApp.Plugins;

public class PizzaPlugin(IHttpClientFactory httpClientFactory)
{
    [KernelFunction("get_pizzas")]
    [Description("Gets all the pizzas in the store.")]
    [return: Description("Json formatted list of all the pizzas in the store.")]
    public async Task<string> GetAllPizzas()
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri($"https://localhost:7233");
        var response = await httpClient.GetAsync("/api/pizza/get-pizzas");
        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }

    [KernelFunction("order_pizza")]
    [Description("Order a pizza from the store based on the pizza name")]
    public async Task<string> OrderPizza(string name)
    {
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri($"https://localhost:7233");
        var response = await httpClient.PostAsync($"/api/pizza/order-pizza/{name}", null);
        var responseContent = await response.Content.ReadAsStringAsync();
        return responseContent;
    }
    
}