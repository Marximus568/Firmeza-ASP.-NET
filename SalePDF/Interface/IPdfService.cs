using SalePDF.DTOs;

namespace SalePDF.Interface;

public interface IPdfService
{
    string GenerateReceiptPdf(SaleReceiptDto data);
}