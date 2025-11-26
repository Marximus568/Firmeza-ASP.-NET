using QuestPDF.Fluent;
using SalePDF.Documents;
using SalePDF.DTOs;
using SalePDF.Interface;

namespace SalePDF.Services;

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
