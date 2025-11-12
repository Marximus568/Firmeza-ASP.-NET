using System.ComponentModel.DataAnnotations;
using AdminDashboard.Application.DTOs.Sales;

namespace AdminDashboardApplication.DTOs.Sales;

public class UpdateSaleDto
{
    [Required]
    public int Id { get; set; }

    public decimal TaxRate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsPaid { get; set; }

    public List<SaleSummaryDto> Items { get; set; } = new();
}