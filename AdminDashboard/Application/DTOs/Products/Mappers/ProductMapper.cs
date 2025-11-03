using AdminDashboard.Domain.Entities;

namespace AdminDashboard.Application.Product.Mappers;

public class ProductMapper
{
    
    public static ProductDto ToDto(Products product)
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

    public static Products ToEntity(CreateProductDto dto)
    {
        return new Products
        {
            Name = dto.Name,
            Description = dto.Description,
            UnitPrice = dto.UnitPrice,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(Products product, UpdateProductDto dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.UnitPrice = dto.UnitPrice;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;
    }
}
