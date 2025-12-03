using QuestPDF.Fluent;
using AdminDashboard.Infrastructure.SalePDF.Documents;
using AdminDashboard.Application.DTOs.SalePDF;
using AdminDashboard.Application.Interfaces.SalePDF;

namespace AdminDashboard.Infrastructure.SalePDF.Services;

public class PdfService : IPdfService
{
    public PdfService()
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
    }

    public string GenerateReceiptPdf(SaleReceiptDto data)
    {
        var document = new SaleReceiptDocument(data);
        var pdfBytes = document.GeneratePdf();
        
        // Convert to base64 for easier handling
        return Convert.ToBase64String(pdfBytes);
    }

    public byte[] GenerateReceiptPdfBytes(SaleReceiptDto data)
    {
        var document = new SaleReceiptDocument(data);
        return document.GeneratePdf();
    }
}
