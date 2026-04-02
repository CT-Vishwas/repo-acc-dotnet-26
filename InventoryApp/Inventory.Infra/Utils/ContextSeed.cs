using Inventory.Core.Config;
using Inventory.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Inventory.Infra.Utils;

public static class ContextSeed
{
    public static async Task SeedRoleAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var adminSettings = serviceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;

        foreach (var role in Enum.GetNames(typeof(UserRole)))
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Seeding the admin
        var adminEmail = adminSettings.Email;
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if(adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = "InventoryAppAdmin",
                Email = adminEmail
            };

            var createAdmin = await userManager.CreateAsync(newAdmin, adminSettings.Password);

            if(createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
            else
            {
                var errors = string.Join(", ", createAdmin.Errors.Select(e=>e.Description));
                throw new Exception($"Seed Admin Failed: {errors}");
            }
        }
    }
}