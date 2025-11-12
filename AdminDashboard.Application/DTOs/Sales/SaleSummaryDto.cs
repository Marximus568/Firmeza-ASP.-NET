namespace AdminDashboard.Application.DTOs.Sales;

using System.ComponentModel.DataAnnotations;

public class SaleSummaryDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    public decimal Subtotal => Quantity * UnitPrice;
}