using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SalePDF.DTOs.Reports;
using SalePDF.Interface;

namespace SalePDF.Services;

public class PdfReportService : IPdfReportService
{
    public PdfReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateProductsReport(List<ProductReportDto> products)
    {
        var document = new ProductsReportDocument(products);
        return document.GeneratePdf();
    }

    public byte[] GenerateClientsReport(List<ClientReportDto> clients)
    {
        var document = new ClientsReportDocument(clients);
        return document.GeneratePdf();
    }

    public byte[] GenerateSalesReport(List<SaleReportDto> sales)
    {
        var document = new SalesReportDocument(sales);
        return document.GeneratePdf();
    }
}
