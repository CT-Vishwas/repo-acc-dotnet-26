namespace Inventory.Core.Entities;

public class Product
{
    public int Id { get; set;}
    public required string Name { get; set;}
    public required double Price { get; set;}
    public int Quantity { get; set;} = 1;

    public ProductCategory Category {get; set;}

    public DateTime CreatedAt {get; set;}
}