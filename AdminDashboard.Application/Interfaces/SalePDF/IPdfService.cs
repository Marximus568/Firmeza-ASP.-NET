using AdminDashboard.Application.DTOs.SalePDF;

namespace AdminDashboard.Application.Interfaces.SalePDF;

public interface IPdfService
{
    string GenerateReceiptPdf(SaleReceiptDto data);
    byte[] GenerateReceiptPdfBytes(SaleReceiptDto data);
}