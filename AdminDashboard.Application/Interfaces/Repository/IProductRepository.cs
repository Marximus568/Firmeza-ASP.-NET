using AdminDashboard.Application.Product;
using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Products;

namespace AdminDashboardApplication.Interfaces.Repository
{
    public interface IProductRepository
    {
        Task<Products> CreateAsync(Products product);
        Task<Products?> GetByIdAsync(int id);
        Task<IEnumerable<Products>> GetAllAsync();
        Task<bool> UpdateAsync(Products product);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Products>> SearchAsync(ProductFilterDto filter);
    }
}