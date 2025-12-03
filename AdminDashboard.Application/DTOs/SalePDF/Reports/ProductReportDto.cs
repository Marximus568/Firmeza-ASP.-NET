namespace AdminDashboard.Application.DTOs.SalePDF.Reports;

public class ProductReportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}
