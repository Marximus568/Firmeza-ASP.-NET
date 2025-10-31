namespace AdminDashboard.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Sales")]
public class Sales
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    [MaxLength(30)]
    public string InvoiceNumber { get; set; } = string.Empty;

    // =============================
    // FOREIGN KEYS
    // =============================

    public int ClientId { get; set; }
    public Client Client { get; set; }

    public int SalerId { get; set; }
    public Saler Saler { get; set; }

    // =============================
    // SALE DETAILS
    // =============================

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    [Column(TypeName = "decimal(10,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; } = 0.19m;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Total { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    public bool IsPaid { get; set; } = true;

    [MaxLength(300)]
    public string Notes { get; set; } = string.Empty;
}