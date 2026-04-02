using Inventory.Core.DTOs.Requests;
using Inventory.Core.DTOs.Responses;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Core.Interfaces;

public interface IAuthService
{
    Task<UserResponseDTO?> LoginAsync(LoginRequestDTO loginRequest);
    Task<IdentityResult> RegisterAsync(UserRequestDTO request);
}