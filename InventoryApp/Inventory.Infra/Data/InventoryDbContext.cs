using Inventory.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infra.Data;

public class InventoryDbContext: IdentityDbContext<IdentityUser>
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options): base(options){}

    public DbSet<Product> Products {get;set;}

    
}