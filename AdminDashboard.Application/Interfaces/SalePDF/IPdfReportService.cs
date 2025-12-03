using AdminDashboard.Application.DTOs.SalePDF.Reports;

namespace AdminDashboard.Application.Interfaces.SalePDF;

public interface IPdfReportService
{
    byte[] GenerateProductsReport(List<ProductReportDto> products);
    byte[] GenerateClientsReport(List<ClientReportDto> clients);
    byte[] GenerateSalesReport(List<SaleReportDto> sales);
}
