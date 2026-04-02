namespace Inventory.Core.DTOs.Requests;

public class LoginRequestDTO
{
    public required string Username { get; set;}
    public required string Password { get; set;}
}