using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.Categories;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public AdminDashboard.Domain.Entities.Categories Category { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Categories.Add(Category);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Category '{Category.Name}' created successfully.";
        return RedirectToPage("./Index");
    }
}
