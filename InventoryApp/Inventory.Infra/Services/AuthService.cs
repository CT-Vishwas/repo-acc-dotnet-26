using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Infra.Services;

public class AuthService : IAuthService
{

    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<UserResponseDTO?> LoginAsync(LoginRequestDTO loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault<string>();
            var token = _tokenService.GenerateToken(user, roles);

            var userresponse = new UserResponseDTO(user.UserName, token);
            return userresponse;
        }

        throw new UnauthorizedAccessException("Invalid Credentials");
    }

    public async Task<IdentityResult> RegisterAsync(UserRequestDTO request)
    {
        var user = new IdentityUser { UserName = request.Username, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "USER");
        }

        return result;
    }
}