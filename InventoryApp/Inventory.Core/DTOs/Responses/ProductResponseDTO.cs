using System.Data.Common;

namespace Inventory.Core.DTOs.Responses;

public class ProductResponseDTO
{
    public int Id {get; set;}
    public required string Name { get; set;}
    public required double Price { get; set;}
    public int Quantity { get; set;} = 1;

    public string Category {get; set;} = string.Empty;
}