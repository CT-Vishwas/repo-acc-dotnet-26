namespace Inventory.Core.DTOs.Requests;

public class ProductRequestDTO
{
    public required string Name { get; set;}
    public required double Price { get; set;}
    public int Quantity { get; set;} = 1;

    public string Category {get; set;} = string.Empty;
}