using AdminDashboard.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Sales;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;
    public EditModel(AppDbContext context) { _context = context; }

    [BindProperty]
    public AdminDashboard.Domain.Entities.Sales Sale { get; set; } = default!;
    public SelectList Clients { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();
        Sale = await _context.Sales.FirstOrDefaultAsync(m => m.Id == id);
        if (Sale == null) return NotFound();
        Clients = new SelectList(await _context.Users.ToListAsync(), "Id", "FirstName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Clients = new SelectList(await _context.Users.ToListAsync(), "Id", "FirstName");
            return Page();
        }
        _context.Attach(Sale).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Venta actualizada exitosamente.";
        return RedirectToPage("./Index");
    }
}
