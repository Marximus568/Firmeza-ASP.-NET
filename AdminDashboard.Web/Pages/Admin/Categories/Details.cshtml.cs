using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Categories;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public AdminDashboard.Domain.Entities.Categories Category { get; set; } = default!;

    public int ProductCount { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        Category = category;
        ProductCount = category.Products?.Count ?? 0;
        return Page();
    }
}
