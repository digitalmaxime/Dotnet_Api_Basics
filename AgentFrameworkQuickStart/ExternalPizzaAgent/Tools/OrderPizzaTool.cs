using System.ComponentModel;

namespace ExternalPizzaAgent.Tools;

public class OrderPizzaTool
{
    [Description("Use this function to order pizza")]
    public static string OrderPizza ()
    {
        return "Pizza ordered!";
    }
}