using AdminDashboard.Domain.Models;

namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Clients")]
public class Clients : Person
{
    
    [MaxLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Role { get; set; } = "Customer";
    
    // Navigation property: One client can have many sales
    public ICollection<Sales> Sales { get; set; } = new List<Sales>();
    
    // Logic stays in Domain
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public Clients(string firstName, string lastName, string email, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phone;
    }
    
    public Clients(
        string firstName,
        string lastName,
        string email,
       
        string phoneNumber,
        string address,
        string role = "Customer"
    )
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        Role = role;
    }

    public Clients()
    {
        throw new NotImplementedException();
    }
}

