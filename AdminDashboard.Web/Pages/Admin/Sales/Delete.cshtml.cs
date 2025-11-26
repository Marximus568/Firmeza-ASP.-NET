using AdminDashboard.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Sales;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;
    public DeleteModel(AppDbContext context) { _context = context; }

    [BindProperty]
    public AdminDashboard.Domain.Entities.Sales Sale {get;set;} = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();
        Sale = await _context.Sales.Include(s => s.Clients).FirstOrDefaultAsync(m => m.Id == id);
        if (Sale == null) return NotFound();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null) return NotFound();
        var sale = await _context.Sales.FindAsync(id);
        if (sale != null) { _context.Sales.Remove(sale); await _context.SaveChangesAsync(); }
        TempData["SuccessMessage"] = "Venta eliminada exitosamente.";
        return RedirectToPage("./Index");
    }
}
