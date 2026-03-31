using Inventory.Core.Entities;
using Inventory.Core.Interfaces;

namespace Inventory.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    public Task<Product> AddAsync(Product product)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Product> GetAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Product> UpdateAsync(Product product)
    {
        throw new NotImplementedException();
    }
}