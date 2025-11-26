namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Table
public class SaleItems
{
    [Key]
    public int Id { get; set; }

    // Quantity of product sold
    public int Quantity { get; set; }

    // Unit price at the time of sale
    [Column(TypeName = "decimal(10,2)")]
    public decimal UnitPrice { get; set; }

    // Subtotal for this item
    [NotMapped]
    public decimal Subtotal => Quantity * UnitPrice;

    // =============================
    // FOREIGN KEYS
    // =============================

    // Foreign Key: Sales
    [ForeignKey("Sales")]
    public int SalesId { get; set; }
    public Sales Sales { get; set; }

    // Foreign Key: Product
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Products Products { get; set; }
}
