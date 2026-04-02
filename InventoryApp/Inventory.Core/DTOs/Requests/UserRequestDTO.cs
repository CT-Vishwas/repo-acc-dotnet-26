namespace Inventory.Core.DTOs.Requests;

public class UserRequestDTO
{
    public required string Username {get; set;}
    public string Password { get; set;}
    public string Email{get; set;}
}