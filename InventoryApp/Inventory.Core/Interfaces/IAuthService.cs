using Inventory.Core.DTOs.Requests;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Core.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterAsync(UserRequestDTO request);
}