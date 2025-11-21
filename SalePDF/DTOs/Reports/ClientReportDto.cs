namespace SalePDF.DTOs.Reports;

public class ClientReportDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int TotalSales { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string FullName => $"{FirstName} {LastName}".Trim();
}
