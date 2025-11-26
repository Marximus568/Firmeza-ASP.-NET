using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Categories;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public AdminDashboard.Domain.Entities.Categories Category { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var category = await _context.Categories.FirstOrDefaultAsync(m => m.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        Category = category;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Category).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(Category.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        TempData["SuccessMessage"] = $"Category '{Category.Name}' updated successfully.";
        return RedirectToPage("./Index");
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
