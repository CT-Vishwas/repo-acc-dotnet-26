using Inventory.Infra.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Inventory.API.Middlewares;

public class ApiKeyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyHeaderName = "X-API-KEY";
    private readonly IConfiguration _configuration;

   private readonly InventoryDbContext _dbContext;

    public ApiKeyAuthHandler(InventoryDbContext inventoryDbContext, IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
        : base(options, logger, encoder, clock)
    {
        _configuration = configuration;
        _dbContext = inventoryDbContext;
    }

    // To generate a secure key
    public string secureKey = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

    // Use a fast but secure hashing algorithm (like SHA256 or HMAC)
    public string HashKey(string key)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(bytes);
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeaderValues))
            return AuthenticateResult.Fail("API Key missing.");

        var providedKey = apiKeyHeaderValues.FirstOrDefault();
        var hashedProvidedKey = HashKey(providedKey); // Helper method from above

        // Query your DB for a match
        var keyRecord = await _dbContext.UserApikeys
            .Include(u => u.UserId)
            .FirstOrDefaultAsync(k => k.ApiKeyHash == hashedProvidedKey && k.IsActive);

        if (keyRecord == null)
            return AuthenticateResult.Fail("Invalid or revoked API Key.");

        // Identity is now tied to the specific user from the DB
        var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, keyRecord.UserId.ToString()),
        new Claim(ClaimTypes.Name, keyRecord.User.Email ?? "Unknown")
    };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}