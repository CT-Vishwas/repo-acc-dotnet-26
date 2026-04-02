namespace Inventory.Core.DTOs.Responses;

public class UserResponseDTO
{
    public string Username { get;set;}
    public string Token { get; set;} = string.Empty;

    public UserResponseDTO(string username, string token)
    {
        Username = username;
        Token = token;
    }
}