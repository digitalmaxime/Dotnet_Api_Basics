using System.ComponentModel;

namespace ExternalPizzaAgent.Tools;

public static class OrderPizzaTool
{
    [Description("Use this function to order pizza")]
    public static string OrderPizza ()
    {
        Console.WriteLine("--- Order Pizza Tool Call ---");
        return "Pizza ordered!";
    }
}