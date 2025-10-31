namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; }

    [MaxLength(300)]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    // Quantity available in inventory
    public int Stock { get; set; }

    // Navigation property: One product can be sold in many sale items
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
