using AdminDashboard.Domain.Entities;
using AdminDashboard.Domain.Interfaces.Repository;
using AdminDashboard.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Infrastructure.Repositories.Customer
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerAsync(Clients customer)
        {
            _context.Users.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(c => c.Email == email);
        }

        public async Task<Clients?> GetByIdAsync(int id)    
        {
            return await _context.Users.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}