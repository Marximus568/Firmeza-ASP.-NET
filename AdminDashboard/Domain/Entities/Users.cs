namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Users")]
public class Users : Person
{
    
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string role { get; set; } = string.Empty;

    // Navigation property: One client can have many sales
    public ICollection<Sales> Sales { get; set; } = new List<Sales>();
}

