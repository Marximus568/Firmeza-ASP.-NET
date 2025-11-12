namespace AdminDashboardApplication.DTOs.SaleItems.Mappers;

public static class SaleItemMapper
{
    // Converts from Entity to DTO
    public static SaleItemDto ToDto(AdminDashboard.Domain.Entities.SaleItems entity)
    {
        return new SaleItemDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductName = entity.Products?.Name, // Avoid null reference
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
            Subtotal = entity.Quantity * entity.UnitPrice,
            SalesId = entity.SalesId
        };
    }

    // Converts from Create DTO to Entity
    public static AdminDashboard.Domain.Entities.SaleItems ToEntity(CreateSaleItemDto dto, decimal unitPrice)
    {
        return new AdminDashboard.Domain.Entities.SaleItems()
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UnitPrice = unitPrice,
            SalesId = dto.SalesId
        };
    }
}