using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Core.Entities;

public class ApiKey
{
    public Guid Id {get; set;}
    public string UserId {get;set;} 
    public string ApiKeyHash {get; set;}

    [ForeignKey("UserId")]
    public virtual IdentityUser User {get;set;}
    public bool IsActive {get; set;}
    public DateTime CreatedAt { get; set;}
}