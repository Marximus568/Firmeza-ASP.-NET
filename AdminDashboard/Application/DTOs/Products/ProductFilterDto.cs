namespace AdminDashboard.Application.Product;

public class ProductFilterDto
{
    public string? SearchTerm { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
    public int? CategoryId { get; set; }
}