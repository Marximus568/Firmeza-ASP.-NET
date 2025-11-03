using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Application.Product;

public class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "Description cannot exceed 300 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Unit price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999999.99")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    public int? CategoryId { get; set; }
}