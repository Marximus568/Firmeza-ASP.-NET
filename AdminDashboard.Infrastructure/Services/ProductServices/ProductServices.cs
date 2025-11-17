using AdminDashboard.Application.Product;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using AdminDashboardApplication.DTOs.Products.Mappers;
using AdminDashboardApplication.Interfaces.Repository;

namespace AdminDashboard.Infrastructure.Services.ProductServices
{
    public class ProductService : IProductServices
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
        {
            var entity = ProductMapper.ToEntity(createDto);
            var created = await _repository.CreateAsync(entity);
            return ProductMapper.ToDto(created);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product != null ? ProductMapper.ToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();
            return products.Select(ProductMapper.ToDto);
        }

        public async Task<bool> UpdateAsync(UpdateProductDto updateDto)
        {
            var product = await _repository.GetByIdAsync(updateDto.Id);
            if (product == null) return false;

            ProductMapper.UpdateEntity(product, updateDto);
            return await _repository.UpdateAsync(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(ProductFilterDto filter)
        {
            var products = await _repository.SearchAsync(filter);
            return products.Select(ProductMapper.ToDto);
        }
    }
}