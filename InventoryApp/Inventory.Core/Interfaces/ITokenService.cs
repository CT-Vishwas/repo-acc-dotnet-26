namespace Inventory.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

public interface ITokenService
{
    string GenerateToken(IdentityUser user, IList<string> roles);
}