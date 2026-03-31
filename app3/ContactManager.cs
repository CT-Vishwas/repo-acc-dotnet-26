using System.Collections.Generic;
using System.Linq;
using app3.Models;

namespace app3;

public class ContactManager
{
    private List<Contact> contacts = new List<Contact>();

    public void AddContact(Contact contact) => contacts.Add(contact);

    public List<Contact> GetAllContacts() => contacts;

    public void DeleteContact(string name)
    {
        var contact = contacts.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if(contact != null) contacts.Remove(contact);
    }

    public Contact? FindContact(string name)=> contacts.FirstOrDefault(c=> c.Name.Equals(name,StringComparison.OrdinalIgnoreCase));

}