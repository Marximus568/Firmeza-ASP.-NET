namespace SalePDF.DTOs;

public class SaleReceiptDto
{
    public int SaleId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    
    // Customer data
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }

    // Product list
    public List<SaleProductDto> Products { get; set; } = new();

    // Totals
    public decimal Subtotal { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
}