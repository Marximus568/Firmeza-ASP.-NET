using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Infrastructure.Repositories.ProductRepository
{
    
    public class ProductsRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Products> CreateAsync(Products product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return await _context.Products
                .Include(p => p.Category)
                .FirstAsync(p => p.Id == product.Id);
        }

        public async Task<Products?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Products>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateAsync(Products product)
        {
            try
            {
                _context.Products.Update(product);
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

        public async Task<IEnumerable<Products>> SearchAsync(ProductFilterDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Search term (name or description)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)));
            }

            // Price range
            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.UnitPrice >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.UnitPrice <= filter.MaxPrice.Value);

            // Stock range
            if (filter.MinStock.HasValue)
                query = query.Where(p => p.Stock >= filter.MinStock.Value);

            if (filter.MaxStock.HasValue)
                query = query.Where(p => p.Stock <= filter.MaxStock.Value);

            // Category
            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
