using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;

namespace AdminDashboard.Pages.Admin
{
    [Authorize]
    public partial class IndexModel : AdminPageModel
    {
        
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int TotalProducts { get; set; }
        public int TotalClients { get; set; }
        public int TotalSales { get; set; }

        public async Task OnGetAsync()
        {
            TotalProducts = await _context.Products.CountAsync();
            TotalClients = await _context.Users.CountAsync();
            TotalSales = await _context.Sales.CountAsync();
        }
    }
}