using AdminDashboard.Domain.Entities;

namespace AdminDashboardApplication.Interfaces.Repository;

public interface ICustomerRepository
{
    // Add a new customer
    Task AddCustomerAsync(Clients customer);

    // Check if email already exists
    Task<bool> EmailExistsAsync(string email);

    // Optional: Get by ID
    Task<Clients?> GetByIdAsync(int id);
    
    // Get all customers
    Task<List<Clients>> GetAllAsync();
}