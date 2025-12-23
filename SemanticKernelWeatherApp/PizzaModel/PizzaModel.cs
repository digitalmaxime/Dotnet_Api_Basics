namespace PizzaModel;

public class Pizza
{
    public int Quantity { get; set; } = 1;
    public PizzaSize Size { get; set; }
    public float Price { get; set; }
    public string Name { get; set; } = "Pizza";
}

public enum PizzaSize
{
    Small, Medium, Large
}