using Inventory.Core.Entities;

namespace Inventory.Core.DTOs.Responses;

public class ApiKeyResponse
{
    public string Message {get;set;} = "Copy this key now. It will never be shown aganin";
    public string ApiKey {get;set;} = string.Empty;
    public string KeyName { get;set;} = string.Empty;

    public ApiKeyResponse(string keyname, string apiKey)
    {
        ApiKey = apiKey;
        KeyName = keyname;
    }
}