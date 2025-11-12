using AdminDashboard.Application.DTOs.Sales;

namespace AdminDashboardApplication.DTOs.Sales;

public class SaleResponseDto
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public int SalerId { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public string Notes { get; set; } = string.Empty;

    public List<SaleSummaryDto> Items { get; set; } = new();
}