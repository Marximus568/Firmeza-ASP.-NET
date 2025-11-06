using AdminDashboard.Application.Product;

namespace AdminDashboardApplication.DTOs.Products.Mappers;

public class ProductMapper
{
    
    public static ProductDto ToDto(AdminDashboard.Domain.Entities.Products product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            Stock = product.Stock,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static AdminDashboard.Domain.Entities.Products ToEntity(CreateProductDto dto)
    {
        return new AdminDashboard.Domain.Entities.Products
        {
            Name = dto.Name,
            Description = dto.Description,
            UnitPrice = dto.UnitPrice,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(AdminDashboard.Domain.Entities.Products product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.UnitPrice = dto.UnitPrice;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;
    }
}
