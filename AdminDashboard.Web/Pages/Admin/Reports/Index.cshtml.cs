using AdminDashboard.Domain.Entities;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using AdminDashboardApplication.DTOs.Users.Interface;
using AdminDashboardApplication.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AdminDashboard.Application.DTOs.SalePDF.Reports;
using AdminDashboard.Application.Interfaces.SalePDF;

namespace AdminDashboard.Pages.Admin.Reports;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly IPdfReportService _pdfReportService;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly AdminDashboard.Infrastructure.Persistence.Context.AppDbContext _context;

    public IndexModel(
        IPdfReportService pdfReportService,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        AdminDashboard.Infrastructure.Persistence.Context.AppDbContext context)
    {
        _pdfReportService = pdfReportService;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _context = context;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetDownloadProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        
        var productReports = products.Select(p => new ProductReportDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CategoryName = p.Category?.Name ?? "Sin categoría",
            UnitPrice = p.UnitPrice,
            Stock = p.Stock,
            CreatedAt = p.CreatedAt
        }).ToList();

        var pdfBytes = _pdfReportService.GenerateProductsReport(productReports);
        
        return File(pdfBytes, "application/pdf", $"Reporte_Productos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
    }

    public async Task<IActionResult> OnGetDownloadClientsAsync()
    {
        var clients = await _customerRepository.GetAllAsync();
        
        var clientReports = clients.Select(c => new ClientReportDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            TotalSales = c.Sales?.Count ?? 0,
            CreatedAt = DateTime.UtcNow // Using current date since entity doesn't have creation timestamp
        }).ToList();

        var pdfBytes = _pdfReportService.GenerateClientsReport(clientReports);
        
        return File(pdfBytes, "application/pdf", $"Reporte_Clientes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
    }

    public async Task<IActionResult> OnGetDownloadSalesAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.Clients)
            .Include(s => s.Items)
            .ToListAsync();
        
        var salesReports = sales.Select(s => new SaleReportDto
        {
            Id = s.Id,
            InvoiceNumber = s.InvoiceNumber,
            SaleDate = s.SaleDate,
            ClientName = s.Clients?.FullName ?? "N/A",
            Subtotal = s.Subtotal,
            TaxRate = s.TaxRate,
            Total = s.Total,
            PaymentMethod = s.PaymentMethod,
            IsPaid = s.IsPaid,
            ItemCount = s.Items?.Count ?? 0
        }).ToList();

        var pdfBytes = _pdfReportService.GenerateSalesReport(salesReports);
        
        return File(pdfBytes, "application/pdf", $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
    }
}
