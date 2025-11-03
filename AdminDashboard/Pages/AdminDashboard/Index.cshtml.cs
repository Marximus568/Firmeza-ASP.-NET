using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Infrastructure.Persistence.Context;

namespace AdminDashboard.Pages.AdminDashboard
{
    public partial class IndexModel : PageModel
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