namespace SalePDF.DTOs;

public class SaleProductDto
{
    public string Name { get; set; }
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Qty * UnitPrice;
}