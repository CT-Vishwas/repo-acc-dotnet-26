namespace app3.Models;

public class Contact
{
    public string Name { get; set;}
    public string Phone {get; set;}
    public string Email { get; set;}

    public override string ToString()
    {
        return $"Name: {Name}|Phone: {Phone}|Email: {Email}";
    }
}