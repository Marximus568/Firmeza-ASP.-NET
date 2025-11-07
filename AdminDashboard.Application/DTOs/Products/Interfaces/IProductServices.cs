using AdminDashboard.Application.Product;

namespace AdminDashboardApplication.DTOs.Products.Interfaces;

public interface IProductServices
{
    /// <summary>
    /// Creates a new product.
    /// </summary>
    Task<ProductDto> CreateAsync(CreateProductDto createDto);

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    Task<ProductDto?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    Task<IEnumerable<ProductDto>> GetAllAsync();

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    Task<bool> UpdateAsync(UpdateProductDto updateDto);

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Searches and filters products based on criteria.
    /// </summary>
    Task<IEnumerable<ProductDto>> SearchAsync(ProductFilterDto filter);

}