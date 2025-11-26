using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Added this using directive

namespace AdminDashboard.Pages.Admin.Sales;

/// <summary>
/// Page model for listing and searching sales.
/// </summary>
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger; // Added this field

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger) // Modified constructor
    {
        _context = context;
        _logger = logger; // Initialized logger
    }

    public IList<AdminDashboard.Domain.Entities.Sales> Sales { get; set; } = new List<AdminDashboard.Domain.Entities.Sales>();

    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentStatus { get; set; } // Changed from IsPaid to PaymentStatus

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<AdminDashboard.Domain.Entities.Sales> query = _context.Sales
            .Include(s => s.Clients);

        if (!string.IsNullOrEmpty(SearchString))
        {
            query = query.Where(s => s.InvoiceNumber.Contains(SearchString) ||
                                    s.Clients.FirstName.Contains(SearchString) ||
                                    s.Clients.LastName.Contains(SearchString));
        }

        if (!string.IsNullOrEmpty(PaymentStatus))
        {
            if (PaymentStatus == "paid")
            {
                query = query.Where(s => s.IsPaid);
            }
            else if (PaymentStatus == "pending")
            {
                query = query.Where(s => !s.IsPaid);
            }
        }

        Sales = await query
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }
}
