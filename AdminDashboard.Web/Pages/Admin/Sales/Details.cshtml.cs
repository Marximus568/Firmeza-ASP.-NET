using AdminDashboard.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Pages.Admin.Sales;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;
    public DetailsModel(AppDbContext context) { _context = context; }

    public AdminDashboard.Domain.Entities.Sales Sale { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null) return NotFound();
        Sale = await _context.Sales.Include(s => s.Clients).FirstOrDefaultAsync(m => m.Id == id);
        if (Sale == null) return NotFound();
        return Page();
    }
}
