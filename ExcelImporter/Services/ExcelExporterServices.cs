using AdminDashboard.Infrastructure.Persistence.Context;
using ExcelImporter.Interfaces;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ExcelImporter.Services;

public class ExcelExporterServices : IExcelExporter
{
    private readonly AppDbContext _context;

    public ExcelExporterServices(AppDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> ExportClientsAsync()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Clients");

        // Header row
        ws.Cells[1, 1].Value = "FirstName";
        ws.Cells[1, 2].Value = "LastName";
        ws.Cells[1, 3].Value = "Email";
        ws.Cells[1, 4].Value = "PhoneNumber";
        ws.Cells[1, 5].Value = "Address";
        ws.Cells[1, 6].Value = "Role";

        var clients = await _context.Users.ToListAsync();

        int row = 2;
        foreach (var c in clients)
        {
            ws.Cells[row, 1].Value = c.FirstName;
            ws.Cells[row, 2].Value = c.LastName;
            ws.Cells[row, 3].Value = c.Email;
            ws.Cells[row, 4].Value = c.PhoneNumber;
            ws.Cells[row, 5].Value = c.Address;
            ws.Cells[row, 6].Value = c.Role;
            row++;
        }

        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportProductsAsync()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Products");

        ws.Cells[1, 1].Value = "Name";
        ws.Cells[1, 2].Value = "Description";
        ws.Cells[1, 3].Value = "UnitPrice";
        ws.Cells[1, 4].Value = "Stock";
        ws.Cells[1, 5].Value = "CategoryId";

        var products = await _context.Products.ToListAsync();

        int row = 2;
        foreach (var p in products)
        {
            ws.Cells[row, 1].Value = p.Name;
            ws.Cells[row, 2].Value = p.Description;
            ws.Cells[row, 3].Value = p.UnitPrice;
            ws.Cells[row, 4].Value = p.Stock;
            ws.Cells[row, 5].Value = p.CategoryId;
            row++;
        }

        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportSalesAsync()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("Sales");

        ws.Cells[1, 1].Value = "InvoiceNumber";
        ws.Cells[1, 2].Value = "SaleDate";
        ws.Cells[1, 3].Value = "ClientId";
        ws.Cells[1, 4].Value = "Subtotal";
        ws.Cells[1, 5].Value = "TaxRate";
        ws.Cells[1, 6].Value = "Total";
        ws.Cells[1, 7].Value = "PaymentMethod";
        ws.Cells[1, 8].Value = "IsPaid";
        ws.Cells[1, 9].Value = "Notes";

        var sales = await _context.Sales.ToListAsync();

        int row = 2;
        foreach (var s in sales)
        {
            ws.Cells[row, 1].Value = s.InvoiceNumber;
            ws.Cells[row, 2].Value = s.SaleDate;
            ws.Cells[row, 3].Value = s.ClientId;
            ws.Cells[row, 4].Value = s.Subtotal;
            ws.Cells[row, 5].Value = s.TaxRate;
            ws.Cells[row, 6].Value = s.Total;
            ws.Cells[row, 7].Value = s.PaymentMethod;
            ws.Cells[row, 8].Value = s.IsPaid;
            ws.Cells[row, 9].Value = s.Notes;
            row++;
        }

        return package.GetAsByteArray();
    }

    public async Task<byte[]> ExportSaleItemsAsync()
    {
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("SaleItems");

        ws.Cells[1, 1].Value = "SalesId";
        ws.Cells[1, 2].Value = "ProductId";
        ws.Cells[1, 3].Value = "Quantity";
        ws.Cells[1, 4].Value = "UnitPrice";

        var items = await _context.SaleItems.ToListAsync();

        int row = 2;
        foreach (var si in items)
        {
            ws.Cells[row, 1].Value = si.SalesId;
            ws.Cells[row, 2].Value = si.ProductId;
            ws.Cells[row, 3].Value = si.Quantity;
            ws.Cells[row, 4].Value = si.UnitPrice;
            row++;
        }

        return package.GetAsByteArray();
    }
}