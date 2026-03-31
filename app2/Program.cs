
namespace App2;
public record Product(int Id, string Name, int Quantity, decimal Price);

public class Program
{
    public static void Main()
    {
        Product[] inventory =
        {
            new Product(1,"Laptop", 2, 25000.00m),
            new Product(2,"Mouse", 15, 250.00m) ,
            new Product(3,"Keyboard", 3,150.00m) ,
            new Product(4,"Monitor", 10, 60000.00m) 
        };

        // Finding low stock items 
        var lowStockItems = inventory.Where(p=> p.Quantity < 5);

        decimal totalValue = lowStockItems.Sum(p=> p.Price * p.Quantity);

        Console.WriteLine($"Low Stock Products:");

        foreach (var item in lowStockItems)
        {
            Console.WriteLine($"- {item.Name}: {item.Quantity}");
        }

        Console.WriteLine($"Total value to restock: {totalValue:C2}");
    }
}