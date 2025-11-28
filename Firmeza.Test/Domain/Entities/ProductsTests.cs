using AdminDashboard.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Firmeza.Test.Domain.Entities;

public class ProductsTests
{
    [Fact]
    public void Product_ShouldBeCreatedWithDefaultValues()
    {
        // Arrange & Act
        var product = new Products();

        // Assert
        product.Should().NotBeNull();
        product.Stock.Should().Be(0);
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        product.SaleItems.Should().NotBeNull();
        product.SaleItems.Should().BeEmpty();
    }

    [Fact]
    public void Product_WithRequiredProperties_ShouldSetCorrectly()
    {
        // Arrange
        var name = "Test Product";
        var description = "Test Description";
        var unitPrice = 99.99m;
        var stock = 50;

        // Act
        var product = new Products
        {
            Name = name,
            Description = description,
            UnitPrice = unitPrice,
            Stock = stock
        };

        // Assert
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.UnitPrice.Should().Be(unitPrice);
        product.Stock.Should().Be(stock);
    }

    [Fact]
    public void Product_UnitPrice_ShouldHandleDecimalPrecision()
    {
        // Arrange
        var product = new Products();
        var price = 123.45m;

        // Act
        product.UnitPrice = price;

        // Assert
        product.UnitPrice.Should().Be(price);
    }

    [Fact]
    public void Product_Stock_CanBeZero()
    {
        // Arrange & Act
        var product = new Products { Stock = 0 };

        // Assert
        product.Stock.Should().Be(0);
    }

    [Fact]
    public void Product_Stock_CanBeNegative()
    {
        // Arrange & Act
        var product = new Products { Stock = -5 };

        // Assert
        product.Stock.Should().Be(-5);
    }

    [Fact]
    public void Product_CategoryId_CanBeNull()
    {
        // Arrange & Act
        var product = new Products { CategoryId = null };

        // Assert
        product.CategoryId.Should().BeNull();
    }

    [Fact]
    public void Product_CategoryId_CanBeSet()
    {
        // Arrange
        var categoryId = 10;

        // Act
        var product = new Products { CategoryId = categoryId };

        // Assert
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Product_Category_NavigationProperty_CanBeNull()
    {
        // Arrange & Act
        var product = new Products();

        // Assert
        product.Category.Should().BeNull();
    }

    [Fact]
    public void Product_Category_NavigationProperty_CanBeSet()
    {
        // Arrange
        var category = new Categories { Id = 1, Name = "Electronics" };
        var product = new Products { CategoryId = category.Id, Category = category };

        // Act & Assert
        product.Category.Should().NotBeNull();
        product.Category.Name.Should().Be("Electronics");
    }

    [Fact]
    public void Product_UpdatedAt_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var product = new Products();

        // Assert
        product.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Product_UpdatedAt_CanBeSet()
    {
        // Arrange
        var product = new Products();
        var updateTime = DateTime.UtcNow;

        // Act
        product.UpdatedAt = updateTime;

        // Assert
        product.UpdatedAt.Should().Be(updateTime);
    }

    [Fact]
    public void Product_SaleItems_NavigationProperty_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var product = new Products();

        // Assert
        product.SaleItems.Should().NotBeNull();
        product.SaleItems.Should().BeEmpty();
    }

    [Fact]
    public void Product_CanAddMultipleSaleItems()
    {
        // Arrange
        var product = new Products { Id = 1, Name = "Test Product" };
        var saleItem1 = new SaleItems { ProductId = product.Id, Quantity = 2 };
        var saleItem2 = new SaleItems { ProductId = product.Id, Quantity = 3 };

        // Act
        product.SaleItems.Add(saleItem1);
        product.SaleItems.Add(saleItem2);

        // Assert
        product.SaleItems.Should().HaveCount(2);
        product.SaleItems.Should().Contain(saleItem1);
        product.SaleItems.Should().Contain(saleItem2);
    }

    [Fact]
    public void Product_CreatedAt_ShouldBeSetOnCreation()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var product = new Products { Name = "Test Product", UnitPrice = 100 };
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        // Assert
        product.CreatedAt.Should().BeAfter(beforeCreation);
        product.CreatedAt.Should().BeBefore(afterCreation);
    }
}
