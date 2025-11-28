using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Products;
using AutoMapper;
using Firmeza.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Firmeza.Test.WebApi.Controllers;

public class ProductsControllerTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockMapper = new Mock<IMapper>();
        _controller = new ProductsController(_context, _mockMapper.Object);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOkWithListOfProducts()
    {
        // Arrange
        var product = new Products { Id = 1, Name = "Test Product", Description = "Test Description", UnitPrice = 100m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Test Product", Description = "Test Description", UnitPrice = 100m }
        };

        _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Products>>()))
            .Returns(productDtos);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedDtos = actionResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
        returnedDtos.Should().HaveCount(1);
    }

    [Fact]
    public async Task PostProduct_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "New Product", Description = "New Description", UnitPrice = 50m };
        var productEntity = new Products { Id = 1, Name = "New Product", Description = "New Description", UnitPrice = 50m };
        var productDto = new ProductDto { Id = 1, Name = "New Product", Description = "New Description", UnitPrice = 50m };

        _mockMapper.Setup(m => m.Map<Products>(createDto)).Returns(productEntity);
        _mockMapper.Setup(m => m.Map<ProductDto>(productEntity)).Returns(productDto);

        // Act
        var result = await _controller.PostProduct(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedDto = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
        
        returnedDto.Id.Should().Be(1);
        returnedDto.Name.Should().Be("New Product");
        
        // Verify DB
        var dbProduct = await _context.Products.FindAsync(1);
        dbProduct.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProduct_WithExistingId_ShouldReturnOk()
    {
        // Arrange
        var product = new Products { Id = 2, Name = "Existing Product", Description = "Existing Description", UnitPrice = 200m };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var productDto = new ProductDto { Id = 2, Name = "Existing Product", Description = "Existing Description", UnitPrice = 200m };
        _mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Products>())).Returns(productDto);

        // Act
        var result = await _controller.GetProduct(2);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        actionResult.Value.Should().BeOfType<ProductDto>();
    }

    [Fact]
    public async Task GetProduct_WithNonExistingId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetProduct(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}
