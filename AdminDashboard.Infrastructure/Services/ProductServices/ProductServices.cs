using AdminDashboard.Application.Product;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using AdminDashboardApplication.DTOs.Products.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Infrastructure.Services.ProductServices;

 public class ProductService : IProductServices
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto createDto)
        {
            var product = ProductMapper.ToEntity(createDto);
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Reload with category
            var createdProduct = await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == product.Id);

            return ProductMapper.ToDto(createdProduct);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product != null ? ProductMapper.ToDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return products.Select(ProductMapper.ToDto);
        }

        public async Task<bool> UpdateAsync(UpdateProductDto updateDto)
        {
            var product = await _context.Products.FindAsync(updateDto.Id);
            
            if (product == null)
                return false;

            ProductMapper.UpdateEntity(product, updateDto);
            
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(ProductFilterDto filter)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Filter by search term (name or description)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(p => 
                    p.Name.ToLower().Contains(searchTerm) || 
                    (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
            }

            // Filter by price range
            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.UnitPrice <= filter.MaxPrice.Value);
            }

            // Filter by stock range
            if (filter.MinStock.HasValue)
            {
                query = query.Where(p => p.Stock >= filter.MinStock.Value);
            }

            if (filter.MaxStock.HasValue)
            {
                query = query.Where(p => p.Stock <= filter.MaxStock.Value);
            }

            // Filter by category
            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return products.Select(ProductMapper.ToDto);
        }
    }
