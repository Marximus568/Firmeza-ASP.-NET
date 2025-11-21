using AdminDashboard.Application.DTOs.Sales;

namespace AdminDashboardApplication.DTOs.Sales.Mappers;

public static class SalesMapper
{
    // Map from Create DTO to Entity
    public static AdminDashboard.Domain.Entities.Sales ToEntity(CreateSaleDto dto)
    {
        var sale = new AdminDashboard.Domain.Entities.Sales
        {
            SaleDate = DateTime.UtcNow,
            ClientId = dto.ClientId,
            TaxRate = dto.TaxRate,
            PaymentMethod = dto.PaymentMethod,
            Notes = dto.Notes,
            Items = dto.Items.Select(item => new AdminDashboard.Domain.Entities.SaleItems
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        // Calculate totals
        sale.Subtotal = sale.Items.Sum(i => i.Quantity * i.UnitPrice);
        sale.Total = sale.Subtotal + (sale.Subtotal * sale.TaxRate);
        sale.IsPaid = false;

        return sale;
    }

    // Map from Entity to Response DTO
    public static SaleResponseDto ToDto(AdminDashboard.Domain.Entities.Sales entity)
    {
        return new SaleResponseDto
        {
            Id = entity.Id,
            SaleDate = entity.SaleDate,
            InvoiceNumber = entity.InvoiceNumber,
            ClientId = entity.ClientId,
            Subtotal = entity.Subtotal,
            TaxRate = entity.TaxRate,
            Total = entity.Total,
            PaymentMethod = entity.PaymentMethod,
            IsPaid = entity.IsPaid,
            Notes = entity.Notes,
            Items = entity.Items.Select(i => new SaleSummaryDto
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}