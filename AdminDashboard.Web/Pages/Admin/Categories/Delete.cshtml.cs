using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Categories;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (category != null)
        {
            // Check if category has products
            if (category.Products != null && category.Products.Any())
            {
                TempData["ErrorMessage"] = $"Cannot delete category '{category.Name}' because it has {category.Products.Count} associated product(s).";
                return RedirectToPage("./Index");
            }

            Category = category;
            _context.Categories.Remove(Category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category '{Category.Name}' deleted successfully.";
        }

        return RedirectToPage("./Index");
    }
}
