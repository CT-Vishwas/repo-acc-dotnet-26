namespace Inventory.Core.Config;

public class AdminSettings
{
    public const string SectionName = "DefaultAdmin";
    public string Email { get; set;} = string.Empty;
    public string Password { get; set;} = string.Empty;
}