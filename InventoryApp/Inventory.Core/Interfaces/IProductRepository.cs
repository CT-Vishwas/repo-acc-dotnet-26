using Inventory.Core.Entities;

namespace Inventory.Core.Interfaces;

public interface IProductRepository
{
    // AddProduct
    Task<Product> AddAsync(Product product);
    // GetAll products
    Task<IEnumerable<Product>> GetAllAsync();
    //Update product
    Task<Product> UpdateAsync(Product product);
    // Get Single product
    Task<Product?> GetAsync(int id);
    // Delete Product
    Task DeleteAsync(int id);
}