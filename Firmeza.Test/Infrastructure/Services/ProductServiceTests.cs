using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Services.ProductServices;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.Interfaces.Repository;
using FluentAssertions;
using Moq;
using Xunit;

namespace Firmeza.Test.Infrastructure.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateProduct()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "Test Product",
            Description = "Test Description",
            UnitPrice = 99.99m,
            Stock = 50,
            CategoryId = 1
        };

        var createdProduct = new Products
        {
            Id = 1,
            Name = dto.Name,
            Description = dto.Description,
            UnitPrice = dto.UnitPrice,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Products>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.UnitPrice.Should().Be(dto.UnitPrice);
        _mockRepository.Verify(x => x.CreateAsync(It.IsAny<Products>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1;
        var product = new Products
        {
            Id = productId,
            Name = "Test Product",
            UnitPrice = 50m,
            Stock = 10
        };

        _mockRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        _mockRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var productId = 999;
        _mockRepository.Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Products?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Products>
        {
            new Products { Id = 1, Name = "Product 1", UnitPrice = 10m },
            new Products { Id = 2, Name = "Product 2", UnitPrice = 20m },
            new Products { Id = 3, Name = "Product 3", UnitPrice = 30m }
        };

        _mockRepository.Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingProduct_ShouldReturnTrue()
    {
        // Arrange
        var updateDto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Product",
            Description = "Updated Description",
            UnitPrice = 149.99m,
            Stock = 100,
            CategoryId = 2
        };

        var existingProduct = new Products
        {
            Id = 1,
            Name = "Old Product",
            UnitPrice = 99.99m
        };

        _mockRepository.Setup(x => x.GetByIdAsync(updateDto.Id))
            .ReturnsAsync(existingProduct);
        
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Products>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.UpdateAsync(updateDto);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.GetByIdAsync(updateDto.Id), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Products>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistentProduct_ShouldReturnFalse()
    {
        // Arrange
        var updateDto = new UpdateProductDto
        {
            Id = 999,
            Name = "Updated Product"
        };

        _mockRepository.Setup(x => x.GetByIdAsync(updateDto.Id))
            .ReturnsAsync((Products?)null);

        // Act
        var result = await _productService.UpdateAsync(updateDto);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.GetByIdAsync(updateDto.Id), Times.Once);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Products>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var productId = 1;
        _mockRepository.Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        var productId = 999;
        _mockRepository.Setup(x => x.DeleteAsync(productId))
            .ReturnsAsync(false);

        // Act
        var result = await _productService.DeleteAsync(productId);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithFilter_ShouldReturnFilteredProducts()
    {
        // Arrange
        var filter = new ProductFilterDto
        {
            SearchTerm = "laptop"
        };

        var filteredProducts = new List<Products>
        {
            new Products { Id = 1, Name = "Laptop Dell", UnitPrice = 800m },
            new Products { Id = 2, Name = "Laptop HP", UnitPrice = 750m }
        };

        _mockRepository.Setup(x => x.SearchAsync(filter))
            .ReturnsAsync(filteredProducts);

        // Act
        var result = await _productService.SearchAsync(filter);

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(x => x.SearchAsync(filter), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithEmptyFilter_ShouldReturnAllProducts()
    {
        // Arrange
        var filter = new ProductFilterDto();
        var allProducts = new List<Products>
        {
            new Products { Id = 1, Name = "Product 1" },
            new Products { Id = 2, Name = "Product 2" },
            new Products { Id = 3, Name = "Product 3" }
        };

        _mockRepository.Setup(x => x.SearchAsync(filter))
            .ReturnsAsync(allProducts);

        // Act
        var result = await _productService.SearchAsync(filter);

        // Assert
        result.Should().HaveCount(3);
        _mockRepository.Verify(x => x.SearchAsync(filter), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldPassCorrectValuesToRepository()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "New Product",
            Description = "Description",
            UnitPrice = 59.99m,
            Stock = 25,
            CategoryId = 5
        };

        Products? capturedProduct = null;
        _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Products>()))
            .Callback<Products>(p => capturedProduct = p)
            .ReturnsAsync((Products p) => p);

        // Act
        await _productService.CreateAsync(dto);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct!.Name.Should().Be(dto.Name);
        capturedProduct.Description.Should().Be(dto.Description);
        capturedProduct.UnitPrice.Should().Be(dto.UnitPrice);
        capturedProduct.Stock.Should().Be(dto.Stock);
        capturedProduct.CategoryId.Should().Be(dto.CategoryId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldApplyAllChangesToProduct()
    {
        // Arrange
        var existingProduct = new Products
        {
            Id = 1,
            Name = "Old Name",
            Description = "Old Description",
            UnitPrice = 100m,
            Stock = 50,
            CategoryId = 1
        };

        var updateDto = new UpdateProductDto
        {
            Id = 1,
            Name = "New Name",
            Description = "New Description",
            UnitPrice = 200m,
            Stock = 100,
            CategoryId = 2
        };

        _mockRepository.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Products>())).ReturnsAsync(true);

        // Act
        await _productService.UpdateAsync(updateDto);

        // Assert
        existingProduct.Name.Should().Be("New Name");
        existingProduct.Description.Should().Be("New Description");
        existingProduct.UnitPrice.Should().Be(200m);
        existingProduct.Stock.Should().Be(100);
        existingProduct.CategoryId.Should().Be(2);
        existingProduct.UpdatedAt.Should().NotBeNull();
    }
}
