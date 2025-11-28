using AdminDashboardApplication.DTOs.Sales;
using AdminDashboard.Application.DTOs.Sales;
using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Sales.Mappers;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Application.Mappers;

public class SalesMapperTests
{
    [Fact]
    public void ToEntity_WithValidCreateSaleDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 5,
            TaxRate = 0.21m,
            PaymentMethod = "Credit Card",
            Notes = "Express delivery requested",
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 2, UnitPrice = 50m },
                new SaleSummaryDto { ProductId = 2, Quantity = 3, UnitPrice = 30m }
            }
        };

        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = SalesMapper.ToEntity(dto);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        entity.Should().NotBeNull();
        entity.ClientId.Should().Be(dto.ClientId);
        entity.TaxRate.Should().Be(dto.TaxRate);
        entity.PaymentMethod.Should().Be(dto.PaymentMethod);
        entity.Notes.Should().Be(dto.Notes);
        entity.SaleDate.Should().BeAfter(beforeCreation);
        entity.SaleDate.Should().BeBefore(afterCreation);
        entity.Items.Should().HaveCount(2);
        entity.IsPaid.Should().BeFalse(); // Default is false in mapper
    }

    [Fact]
    public void ToEntity_ShouldCalculateSubtotalCorrectly()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 2, UnitPrice = 50m },  // 100
                new SaleSummaryDto { ProductId = 2, Quantity = 3, UnitPrice = 30m }   // 90
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.Subtotal.Should().Be(190m); // 100 + 90
    }

    [Fact]
    public void ToEntity_ShouldCalculateTotalWithTaxCorrectly()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            TaxRate = 0.19m,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 10, UnitPrice = 10m } // 100
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.Subtotal.Should().Be(100m);
        entity.Total.Should().Be(119m); // 100 + (100 * 0.19)
    }

    [Fact]
    public void ToEntity_WithZeroTaxRate_ShouldHaveTotalEqualToSubtotal()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            TaxRate = 0m,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 5, UnitPrice = 20m }
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.Subtotal.Should().Be(100m);
        entity.Total.Should().Be(100m); // No tax applied
    }

    [Fact]
    public void ToEntity_ShouldMapSaleItemsCorrectly()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 10, Quantity = 5, UnitPrice = 25.50m },
                new SaleSummaryDto { ProductId = 20, Quantity = 2, UnitPrice = 100m }
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.Items.Should().HaveCount(2);
        
        var firstItem = entity.Items.First();
        firstItem.ProductId.Should().Be(10);
        firstItem.Quantity.Should().Be(5);
        firstItem.UnitPrice.Should().Be(25.50m);

        var secondItem = entity.Items.Last();
        secondItem.ProductId.Should().Be(20);
        secondItem.Quantity.Should().Be(2);
        secondItem.UnitPrice.Should().Be(100m);
    }

    [Fact]
    public void ToEntity_WithEmptyItems_ShouldHaveZeroSubtotalAndTotal()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            TaxRate = 0.19m,
            Items = new List<SaleSummaryDto>()
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.Items.Should().BeEmpty();
        entity.Subtotal.Should().Be(0m);
        entity.Total.Should().Be(0m);
    }

    [Fact]
    public void ToEntity_ShouldSetIsPaidToFalse()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 1, UnitPrice = 10m }
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        entity.IsPaid.Should().BeFalse();
    }

    [Fact]
    public void ToDto_WithValidSale_ShouldMapCorrectly()
    {
        // Arrange
        var sale = new Sales
        {
            Id = 1,
            SaleDate = DateTime.UtcNow,
            InvoiceNumber = "INV-2025-001",
            ClientId = 5,
            Subtotal = 200m,
            TaxRate = 0.19m,
            Total = 238m,
            PaymentMethod = "Cash",
            IsPaid = true,
            Notes = "Customer picked up in store",
            Items = new List<SaleItems>
            {
                new SaleItems { Id = 1, ProductId = 10, Quantity = 2, UnitPrice = 100m }
            }
        };

        // Act
        var dto = SalesMapper.ToDto(sale);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(sale.Id);
        dto.SaleDate.Should().Be(sale.SaleDate);
        dto.InvoiceNumber.Should().Be(sale.InvoiceNumber);
        dto.ClientId.Should().Be(sale.ClientId);
        dto.Subtotal.Should().Be(sale.Subtotal);
        dto.TaxRate.Should().Be(sale.TaxRate);
        dto.Total.Should().Be(sale.Total);
        dto.PaymentMethod.Should().Be(sale.PaymentMethod);
        dto.IsPaid.Should().BeTrue();
        dto.Notes.Should().Be(sale.Notes);
    }

    [Fact]
    public void ToDto_ShouldMapSaleItemsToSummaryDtos()
    {
        // Arrange
        var sale = new Sales
        {
            Id = 1,
            ClientId = 1,
            Items = new List<SaleItems>
            {
                new SaleItems { ProductId = 10, Quantity = 5, UnitPrice = 25m },
                new SaleItems { ProductId = 20, Quantity = 3, UnitPrice = 50m }
            }
        };

        // Act
        var dto = SalesMapper.ToDto(sale);

        // Assert
        dto.Items.Should().HaveCount(2);
        
        var firstItem = dto.Items.First();
        firstItem.ProductId.Should().Be(10);
        firstItem.Quantity.Should().Be(5);
        firstItem.UnitPrice.Should().Be(25m);

        var secondItem = dto.Items.Last();
        secondItem.ProductId.Should().Be(20);
        secondItem.Quantity.Should().Be(3);
        secondItem.UnitPrice.Should().Be(50m);
    }

    [Fact]
    public void ToDto_WithEmptySaleItems_ShouldReturnEmptyItemsList()
    {
        // Arrange
        var sale = new Sales
        {
            Id = 1,
            ClientId = 1,
            Items = new List<SaleItems>()
        };

        // Act
        var dto = SalesMapper.ToDto(sale);

        // Assert
        dto.Items.Should().NotBeNull();
        dto.Items.Should().BeEmpty();
    }

    [Fact]
    public void ToEntity_WithComplexCalculation_ShouldBeAccurate()
    {
        // Arrange
        var dto = new CreateSaleDto
        {
            ClientId = 1,
            TaxRate = 0.21m,
            Items = new List<SaleSummaryDto>
            {
                new SaleSummaryDto { ProductId = 1, Quantity = 3, UnitPrice = 15.75m },  // 47.25
                new SaleSummaryDto { ProductId = 2, Quantity = 5, UnitPrice = 22.50m },  // 112.50
                new SaleSummaryDto { ProductId = 3, Quantity = 2, UnitPrice = 99.99m }   // 199.98
            }
        };

        // Act
        var entity = SalesMapper.ToEntity(dto);

        // Assert
        var expectedSubtotal = 359.73m; // 47.25 + 112.50 + 199.98
        var expectedTotal = 435.27m;    // 359.73 + (359.73 * 0.21) = 359.73 + 75.5433 = 435.2733
        
        entity.Subtotal.Should().Be(expectedSubtotal);
        entity.Total.Should().BeApproximately(expectedTotal, 0.02m); // Allow small rounding difference
    }
}
