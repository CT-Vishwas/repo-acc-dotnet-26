using System.Security.Cryptography;
namespace Inventory.Infra.Utils;
public static class ApiKeyGenerator
{
    private const string Prefix = "ak_";
    private const int KeyLength = 32; // Length in bytes

    public static string Generate()
    {
        var bytes = new byte[KeyLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        
        // Convert to Base64 and append prefix
        var key = Convert.ToBase64String(bytes)
            .Replace("/", "")
            .Replace("+", "")
            .Replace("=", ""); // Remove non-URL friendly chars
            
        return $"{Prefix}{key}";
    }
}