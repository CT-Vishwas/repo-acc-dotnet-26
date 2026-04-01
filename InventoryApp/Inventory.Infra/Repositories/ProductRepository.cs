using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using Inventory.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infra.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly InventoryDbContext _context;

    public ProductRepository(InventoryDbContext inventoryDbContext)
    {
        _context = inventoryDbContext;
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if(existingProduct is null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct is null)
        {
            throw new KeyNotFoundException("Product Not found");
        }

        _context.Products.Remove(existingProduct);
        await _context.SaveChangesAsync();

        return;
    }






}