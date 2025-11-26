using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalePDF.Interface;
using SalePDF.DTOs;

namespace AdminDashboard.Pages.Admin.Sales;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IPdfService _pdfService;
    private readonly IWebHostEnvironment _env;

    public CreateModel(AppDbContext context, IPdfService pdfService, IWebHostEnvironment env)
    {
        _context = context;
        _pdfService = pdfService;
        _env = env;
    }

    public SelectList Clients { get; set; } = default!;

    [BindProperty]
    public AdminDashboard.Domain.Entities.Sales Sale { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var clients = await _context.Users.ToListAsync();
        Clients = new SelectList(clients, "Id", "FirstName");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            var clients = await _context.Users.ToListAsync();
            Clients = new SelectList(clients, "Id", "FirstName");
            return Page();
        }

        _context.Sales.Add(Sale);
        await _context.SaveChangesAsync();

        // Generate PDF receipt automatically
        await GenerateReceiptPdfAsync(Sale.Id);

        TempData["SuccessMessage"] = $"Sale '{Sale.InvoiceNumber}' created successfully and receipt generated.";
        return RedirectToPage("./Index");
    }

    private async Task GenerateReceiptPdfAsync(int saleId)
    {
        try
        {
            // Get sale with all necessary data
            var sale = await _context.Sales
                .Include(s => s.Clients)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Products)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return;

            // Prepare DTO for PDF generation
            var receiptData = new SaleReceiptDto
            {
                SaleId = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                Date = sale.SaleDate,
                CustomerName = $"{sale.Clients.FirstName} {sale.Clients.LastName}",
                CustomerEmail = sale.Clients.Email,
                Products = sale.Items.Select(item => new SaleProductDto
                {
                    Name = item.Products.Name,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                Subtotal = sale.Subtotal,
                Iva = sale.Total - sale.Subtotal,
                Total = sale.Total
            };

            // Generate PDF bytes
            var pdfBytes = _pdfService.GenerateReceiptPdfBytes(receiptData);

            // Create recibos folder if it doesn't exist
            var receiptsPath = Path.Combine(_env.WebRootPath, "recibos");
            Directory.CreateDirectory(receiptsPath);

            // Save PDF file
            var fileName = $"Recibo_{sale.InvoiceNumber.Replace("/", "-")}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var filePath = Path.Combine(receiptsPath, fileName);
            await System.IO.File.WriteAllBytesAsync(filePath, pdfBytes);

            // Update sale with PDF path
            sale.ReceiptPath = $"/recibos/{fileName}";
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log error but don't fail the sale creation
            Console.WriteLine($"Error generating PDF receipt: {ex.Message}");
        }
    }
}
