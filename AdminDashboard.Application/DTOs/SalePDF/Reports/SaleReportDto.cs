namespace AdminDashboard.Application.DTOs.SalePDF.Reports;

public class SaleReportDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public int ItemCount { get; set; }
}
