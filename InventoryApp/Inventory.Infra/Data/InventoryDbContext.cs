using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infra.Data;

public class InventoryDbContext: DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options): base(options){}

    public DbSet<Product> Products {get;set;}
}