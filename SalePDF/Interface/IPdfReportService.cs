using SalePDF.DTOs.Reports;

namespace SalePDF.Interface;

public interface IPdfReportService
{
    byte[] GenerateProductsReport(List<ProductReportDto> products);
    byte[] GenerateClientsReport(List<ClientReportDto> clients);
    byte[] GenerateSalesReport(List<SaleReportDto> sales);
}
