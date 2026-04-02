using Inventory.Core.Config;
using Microsoft.Extensions.Options;

namespace Inventory.Infra.Services;

public class TokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    // public void UseKy() => 
}