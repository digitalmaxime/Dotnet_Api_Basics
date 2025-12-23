using PizzaModel;

namespace PizzaApp.services;

public interface IPizzaService
{
    IEnumerable<Pizza> GetAllPizzas();
    public Pizza? GetPizzaByName(string name);
    Pizza AddPizza(string name, int quantity);
    Pizza? OrderPizza(Pizza pizza);
}

public class PizzaService : IPizzaService
{
    private readonly List<Pizza> _pizzas =
    [
        new Pizza { Size = PizzaSize.Small, Price = 10, Name = "Pepperoni", Quantity = 2 },
        new Pizza { Size = PizzaSize.Medium, Price = 15, Name = "Hawaiian", Quantity = 3 },
        new Pizza { Size = PizzaSize.Large, Price = 20, Name = "Veggie", Quantity = 1 }
    ];

    public IEnumerable<Pizza> GetAllPizzas() => _pizzas;

    public Pizza? GetPizzaByName(string name) => _pizzas.FirstOrDefault(pizza => pizza.Name == name);

    public Pizza AddPizza(string name, int quantity)
    {
        var pizza = GetPizzaByName(name);
        if (pizza is null)
        {
            pizza = new Pizza { Name = name, Quantity = quantity };
            _pizzas.Add(pizza);
        }
        else
        {
            pizza.Quantity += quantity;
        }
        
        return pizza;
    }

    public Pizza? OrderPizza(Pizza pizza)
    {
        var pizzaIndex = _pizzas.IndexOf(pizza);
        if (pizzaIndex == -1) throw new Exception("Pizza not found");
        if (_pizzas[pizzaIndex].Quantity == 0)
        {
            Console.WriteLine($"The pizza you ordered a {pizza.Name} pizza is not available at this time.");
            return null;
        }

        _pizzas[pizzaIndex].Quantity--;
        Console.WriteLine($"You ordered a {pizza.Name} pizza for ${pizza.Price}");
        return pizza;
    }
}