using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboardApplication.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Infrastructure.Repositories.CustomerRepository
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
        
        public async Task<List<Clients>> GetAllAsync()
        {
            return await _context.Users.Include(c => c.Sales).ToListAsync();
        }
    }
}