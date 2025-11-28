using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.DTOs.Products.Mappers;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Application.Mappers;

public class ProductMapperTests
{
    [Fact]
    public void ToDto_WithValidProduct_ShouldMapCorrectly()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 99.99m,
            Stock = 50,
            CategoryId = 5,
            Category = new Categories { Id = 5, Name = "Electronics" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var dto = ProductMapper.ToDto(product);

        // Assert
        dto.Should().NotBeNull();
        dto.Id.Should().Be(product.Id);
        dto.Name.Should().Be(product.Name);
        dto.Description.Should().Be(product.Description);
        dto.UnitPrice.Should().Be(product.UnitPrice);
        dto.Stock.Should().Be(product.Stock);
        dto.CategoryId.Should().Be(product.CategoryId);
        dto.CategoryName.Should().Be("Electronics");
        dto.CreatedAt.Should().Be(product.CreatedAt);
        dto.UpdatedAt.Should().Be(product.UpdatedAt);
    }

    [Fact]
    public void ToDto_WithNullCategory_ShouldMapWithNullCategoryName()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Test Product",
            UnitPrice = 50m,
            Stock = 10,
            CategoryId = null,
            Category = null
        };

        // Act
        var dto = ProductMapper.ToDto(product);

        // Assert
        dto.CategoryId.Should().BeNull();
        dto.CategoryName.Should().BeNull();
    }

    [Fact]
    public void ToDto_WithNullUpdatedAt_ShouldMapCorrectly()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Test Product",
            UnitPrice = 25.50m,
            UpdatedAt = null
        };

        // Act
        var dto = ProductMapper.ToDto(product);

        // Assert
        dto.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void ToEntity_WithValidCreateProductDto_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            UnitPrice = 149.99m,
            Stock = 100,
            CategoryId = 3
        };

        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var entity = ProductMapper.ToEntity(dto);
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        entity.Should().NotBeNull();
        entity.Name.Should().Be(dto.Name);
        entity.Description.Should().Be(dto.Description);
        entity.UnitPrice.Should().Be(dto.UnitPrice);
        entity.Stock.Should().Be(dto.Stock);
        entity.CategoryId.Should().Be(dto.CategoryId);
        entity.CreatedAt.Should().BeAfter(beforeCreation);
        entity.CreatedAt.Should().BeBefore(afterCreation);
    }

    [Fact]
    public void ToEntity_WithNullCategoryId_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Product Without Category",
            Description = "Description",
            UnitPrice = 75m,
            Stock = 20,
            CategoryId = null
        };

        // Act
        var entity = ProductMapper.ToEntity(dto);

        // Assert
        entity.CategoryId.Should().BeNull();
    }

    [Fact]
    public void UpdateEntity_WithValidDto_ShouldUpdateAllProperties()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description",
            UnitPrice = 50m,
            Stock = 10,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = null
        };

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Name",
            Description = "Updated Description",
            UnitPrice = 99.99m,
            Stock = 50,
            CategoryId = 2
        };

        var beforeUpdate = DateTime.UtcNow.AddSeconds(-1);

        // Act
        ProductMapper.UpdateEntity(product, dto);
        var afterUpdate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        product.Name.Should().Be(dto.Name);
        product.Description.Should().Be(dto.Description);
        product.UnitPrice.Should().Be(dto.UnitPrice);
        product.Stock.Should().Be(dto.Stock);
        product.CategoryId.Should().Be(dto.CategoryId);
        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().BeAfter(beforeUpdate);
        product.UpdatedAt.Should().BeBefore(afterUpdate);
        // CreatedAt should not change
        product.CreatedAt.Should().BeBefore(beforeUpdate);
    }

    [Fact]
    public void UpdateEntity_ShouldNotChangeId()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Product",
            UnitPrice = 100m
        };

        var dto = new UpdateProductDto
        {
            Id = 999, // Different ID
            Name = "Updated Product",
            UnitPrice = 200m
        };

        // Act
        ProductMapper.UpdateEntity(product, dto);

        // Assert
        product.Id.Should().Be(1); // ID should remain unchanged
    }

    [Fact]
    public void UpdateEntity_ShouldNotChangeCreatedAt()
    {
        // Arrange
        var originalCreatedAt = DateTime.UtcNow.AddDays(-30);
        var product = new Products
        {
            Id = 1,
            Name = "Product",
            UnitPrice = 100m,
            CreatedAt = originalCreatedAt
        };

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Product",
            UnitPrice = 200m
        };

        // Act
        ProductMapper.UpdateEntity(product, dto);

        // Assert
        product.CreatedAt.Should().Be(originalCreatedAt);
    }

    [Fact]
    public void UpdateEntity_WithNullCategoryId_ShouldSetToNull()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Product",
            CategoryId = 5
        };

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Product",
            CategoryId = null
        };

        // Act
        ProductMapper.UpdateEntity(product, dto);

        // Assert
        product.CategoryId.Should().BeNull();
    }

    [Fact]
    public void UpdateEntity_ShouldUpdateUpdatedAtTimestamp()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Product",
            UpdatedAt = null
        };

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Product"
        };

        // Act
        ProductMapper.UpdateEntity(product, dto);

        // Assert
        product.UpdatedAt.Should().NotBeNull();
        product.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void ToDto_WithDecimalPrecision_ShouldMaintainPrecision()
    {
        // Arrange
        var product = new Products
        {
            Id = 1,
            Name = "Precision Test",
            UnitPrice = 123.456m // This will be stored as decimal(10,2) = 123.46
        };

        // Act
        var dto = ProductMapper.ToDto(product);

        // Assert
        dto.UnitPrice.Should().Be(product.UnitPrice);
    }
}
