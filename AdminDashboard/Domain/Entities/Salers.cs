namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Salers")]
public class Salers : Person
{
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    public DateTime HireDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    // Navigation property: one saler can have many sales
    public ICollection<Sales> Sales { get; set; } = new List<Sales>();
}