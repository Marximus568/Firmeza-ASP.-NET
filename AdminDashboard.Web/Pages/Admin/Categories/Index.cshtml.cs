using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Categories;

/// <summary>
/// Page model for listing and searching categories.
/// </summary>
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(AppDbContext context, ILogger<IndexModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IList<AdminDashboard.Domain.Entities.Categories> Categories { get; set; } = new List<AdminDashboard.Domain.Entities.Categories>();

    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        IQueryable<AdminDashboard.Domain.Entities.Categories> query = _context.Categories;

        if (!string.IsNullOrEmpty(SearchString))
        {
            query = query.Where(c => c.Name.Contains(SearchString));
        }

        Categories = await query
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
