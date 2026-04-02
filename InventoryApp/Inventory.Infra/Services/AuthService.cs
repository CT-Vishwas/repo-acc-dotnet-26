using Inventory.Core.DTOs.Requests;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Infra.Services;

public class AuthService : IAuthService
{

    private readonly UserManager<IdentityUser> _userManager;

    public AuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> RegisterAsync(UserRequestDTO request)
    {
        var user = new IdentityUser { UserName = request.Username, Email = request.Email};
        var result = await _userManager.CreateAsync(user, request.Password);
        if( result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "USER");
        }

        return result;
    }
}