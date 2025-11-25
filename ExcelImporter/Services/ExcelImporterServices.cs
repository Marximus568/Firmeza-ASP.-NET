using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using ExcelImporter.Interfaces;
using ExcelImporter.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ExcelImporter.Services;

/// <summary>
/// Service for importing clients, products, sales and sale items from Excel files.
/// Includes validation, detailed error reporting, and upsert logic.
/// </summary>
public class ExcelImporterServices : IExcelImporter
{
    private readonly AppDbContext _context;

    public ExcelImporterServices(AppDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    // -------------------------------------------------------------------------
    // CLIENTS IMPORT
    // -------------------------------------------------------------------------

    public async Task<ImportResult> ImportClientsAsync(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (!ValidateClientHeaders(worksheet))
            {
                result.ErrorList.Add(new ImportError
                {
                    RowNumber = 0,
                    Field = "Headers",
                    ErrorMessage = "Invalid headers. Expected: FirstName, LastName, Email, PhoneNumber, Address, Role"
                });
                result.Errors++;
                return result;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // Read Excel values
                    var firstName = worksheet.Cells[row, 1].Text.Trim();
                    var lastName = worksheet.Cells[row, 2].Text.Trim();
                    var email = worksheet.Cells[row, 3].Text.Trim();
                    var phone = worksheet.Cells[row, 4].Text.Trim();
                    var address = worksheet.Cells[row, 5].Text.Trim();
                    var role = worksheet.Cells[row, 6].Text.Trim();

                    // Field-level validation
                    var validationErrors = ValidateClientDetailed(row, firstName, lastName, email, phone, role);
                    if (validationErrors.Any())
                    {
                        result.ErrorList.AddRange(validationErrors);
                        result.Errors += validationErrors.Count;
                        continue;
                    }

                    // Lookup by email
                    var existingClient = await _context.Users
                        .FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());

                    if (existingClient != null)
                    {
                        // UPDATE
                        existingClient.FirstName = firstName;
                        existingClient.LastName = lastName;
                        existingClient.PhoneNumber = phone;
                        existingClient.Address = address;
                        existingClient.Role = string.IsNullOrWhiteSpace(role) ? "Client" : role;

                        result.Updated++;
                    }
                    else
                    {
                        // INSERT
                        var newClient = new Clients(firstName, lastName, email, phone, address,
                            string.IsNullOrWhiteSpace(role) ? "Client" : role);

                        _context.Users.Add(newClient);
                        result.Inserted++;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError
                    {
                        RowNumber = row,
                        Field = "General",
                        ErrorMessage = $"Unexpected error: {ex.Message}"
                    });
                    result.Errors++;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.ErrorList.Add(new ImportError
            {
                RowNumber = 0,
                Field = "System",
                ErrorMessage = $"Critical error: {ex.Message}"
            });
            result.Errors++;
        }

        return result;
    }

    private bool ValidateClientHeaders(ExcelWorksheet sheet)
    {
        var headers = new[] { "FirstName", "LastName", "Email", "PhoneNumber", "Address", "Role" };

        for (int i = 0; i < headers.Length; i++)
        {
            var value = sheet.Cells[1, i + 1].Text.Trim();
            if (!value.Equals(headers[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private List<ImportError> ValidateClientDetailed(int row, string firstName, string lastName, string email, string phone, string role)
    {
        var list = new List<ImportError>();

        // First Name
        if (string.IsNullOrWhiteSpace(firstName))
            list.Add(new(row, "FirstName", "FirstName is required"));
        else if (firstName.Length > 100)
            list.Add(new(row, "FirstName", "FirstName too long (max 100 chars)"));

        // Last Name
        if (string.IsNullOrWhiteSpace(lastName))
            list.Add(new(row, "LastName", "LastName is required"));
        else if (lastName.Length > 100)
            list.Add(new(row, "LastName", "LastName too long (max 100 chars)"));

        // Email
        if (string.IsNullOrWhiteSpace(email))
            list.Add(new(row, "Email", "Email is required"));
        else if (!IsValidEmail(email))
            list.Add(new(row, "Email", "Invalid email format"));

        // Phone
        if (phone?.Length > 15)
            list.Add(new(row, "PhoneNumber", "PhoneNumber too long (max 15 chars)"));

        return list;
    }

    // -------------------------------------------------------------------------
    // PRODUCTS IMPORT
    // -------------------------------------------------------------------------

    public async Task<ImportResult> ImportProductsAsync(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (!ValidateProductHeaders(worksheet))
            {
                result.ErrorList.Add(new ImportError
                {
                    RowNumber = 0,
                    Field = "Headers",
                    ErrorMessage = "Invalid headers. Expected: Name, Description, UnitPrice, Stock, CategoryId"
                });

                result.Errors++;
                return result;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var name = worksheet.Cells[row, 1].Text.Trim();
                    var desc = worksheet.Cells[row, 2].Text.Trim();
                    var priceTxt = worksheet.Cells[row, 3].Text.Trim();
                    var stockTxt = worksheet.Cells[row, 4].Text.Trim();
                    var categoryTxt = worksheet.Cells[row, 5].Text.Trim();

                    // Parse price
                    if (!decimal.TryParse(priceTxt, out var price))
                    {
                        result.ErrorList.Add(new ImportError(row, "UnitPrice", "Invalid number format"));
                        result.Errors++;
                        continue;
                    }

                    // Parse stock
                    if (!int.TryParse(stockTxt, out var stock))
                    {
                        result.ErrorList.Add(new ImportError(row, "Stock", "Invalid number format"));
                        result.Errors++;
                        continue;
                    }

                    // Parse FK
                    int? categoryId = null;
                    if (!string.IsNullOrWhiteSpace(categoryTxt) &&
                        int.TryParse(categoryTxt, out var catParsed))
                        categoryId = catParsed;

                    // Validation
                    var validationErrors = ValidateProductDetailed(row, name, price, stock);
                    if (validationErrors.Any())
                    {
                        result.ErrorList.AddRange(validationErrors);
                        result.Errors += validationErrors.Count;
                        continue;
                    }

                    // Upsert
                    var existing = await _context.Products
                        .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());

                    if (existing != null)
                    {
                        // UPDATE
                        existing.Description = desc;
                        existing.UnitPrice = price;
                        existing.Stock = stock;
                        existing.CategoryId = categoryId;
                        existing.UpdatedAt = DateTime.UtcNow;

                        result.Updated++;
                    }
                    else
                    {
                        // INSERT
                        var newProduct = new Products
                        {
                            Name = name,
                            Description = desc,
                            UnitPrice = price,
                            Stock = stock,
                            CategoryId = categoryId,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Products.Add(newProduct);
                        result.Inserted++;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row, "General", ex.Message));
                    result.Errors++;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.ErrorList.Add(new ImportError(0, "System", $"Critical error: {ex.Message}"));
            result.Errors++;
        }

        return result;
    }

    private bool ValidateProductHeaders(ExcelWorksheet sheet)
    {
        var headers = new[] { "Name", "Description", "UnitPrice", "Stock", "CategoryId" };

        for (int i = 0; i < headers.Length; i++)
        {
            var value = sheet.Cells[1, i + 1].Text.Trim();
            if (!value.Equals(headers[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private List<ImportError> ValidateProductDetailed(int row, string name, decimal price, int stock)
    {
        var list = new List<ImportError>();

        if (string.IsNullOrWhiteSpace(name))
            list.Add(new ImportError(row, "Name", "Name is required"));
        else if (name.Length > 150)
            list.Add(new ImportError(row, "Name", "Name too long (max 150 chars)"));

        if (price < 0)
            list.Add(new ImportError(row, "UnitPrice", "UnitPrice cannot be negative"));

        if (stock < 0)
            list.Add(new ImportError(row, "Stock", "Stock cannot be negative"));

        return list;
    }

    // -------------------------------------------------------------------------
    // SALES IMPORT
    // -------------------------------------------------------------------------

    public async Task<ImportResult> ImportSalesAsync(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var sheet = package.Workbook.Worksheets[0];
            var rowCount = sheet.Dimension?.Rows ?? 0;

            if (!ValidateSalesHeaders(sheet))
            {
                result.ErrorList.Add(new ImportError(0, "Headers",
                    "Expected: InvoiceNumber, SaleDate, ClientId, Subtotal, TaxRate, Total, PaymentMethod, IsPaid, Notes"));

                result.Errors++;
                return result;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var invoice = sheet.Cells[row, 1].Text.Trim();
                    var saleDateTxt = sheet.Cells[row, 2].Text.Trim();
                    var clientIdTxt = sheet.Cells[row, 3].Text.Trim();
                    var subtotalTxt = sheet.Cells[row, 4].Text.Trim();
                    var taxTxt = sheet.Cells[row, 5].Text.Trim();
                    var totalTxt = sheet.Cells[row, 6].Text.Trim();
                    var payment = sheet.Cells[row, 7].Text.Trim();
                    var isPaidTxt = sheet.Cells[row, 8].Text.Trim();
                    var notes = sheet.Cells[row, 9].Text.Trim();

                    // Parse date
                    DateTime saleDate = DateTime.TryParse(saleDateTxt, out var parsed) ? parsed : DateTime.UtcNow;

                    // Parse client
                    if (!int.TryParse(clientIdTxt, out var clientId))
                    {
                        result.ErrorList.Add(new ImportError(row, "ClientId", "Invalid ClientId"));
                        result.Errors++;
                        continue;
                    }

                    // Parse subtotal
                    if (!decimal.TryParse(subtotalTxt, out var subtotal))
                    {
                        result.ErrorList.Add(new ImportError(row, "Subtotal", "Invalid Subtotal"));
                        result.Errors++;
                        continue;
                    }

                    // Parse tax rate
                    decimal taxRate = decimal.TryParse(taxTxt, out var parsedTax)
                        ? parsedTax
                        : 0.19m;

                    // Parse total
                    if (!decimal.TryParse(totalTxt, out var total))
                    {
                        result.ErrorList.Add(new ImportError(row, "Total", "Invalid Total"));
                        result.Errors++;
                        continue;
                    }

                    // Parse boolean
                    bool isPaid = isPaidTxt.Equals("true", StringComparison.OrdinalIgnoreCase)
                               || isPaidTxt.Equals("1");

                    // Validate FK
                    bool clientExists = await _context.Users.AnyAsync(c => c.Id == clientId);
                    if (!clientExists)
                    {
                        result.ErrorList.Add(new ImportError(row, "ClientId", $"Client with Id {clientId} does not exist"));
                        result.Errors++;
                        continue;
                    }

                    // Upsert
                    var existing = await _context.Sales
                        .FirstOrDefaultAsync(s => s.InvoiceNumber == invoice);

                    if (existing != null)
                    {
                        // UPDATE
                        existing.SaleDate = saleDate;
                        existing.ClientId = clientId;
                        existing.Subtotal = subtotal;
                        existing.TaxRate = taxRate;
                        existing.Total = total;
                        existing.PaymentMethod = payment;
                        existing.IsPaid = isPaid;
                        existing.Notes = notes;

                        result.Updated++;
                    }
                    else
                    {
                        // INSERT
                        var newSale = new Sales
                        {
                            InvoiceNumber = invoice,
                            SaleDate = saleDate,
                            ClientId = clientId,
                            Subtotal = subtotal,
                            TaxRate = taxRate,
                            Total = total,
                            PaymentMethod = payment,
                            IsPaid = isPaid,
                            Notes = notes
                        };

                        _context.Sales.Add(newSale);
                        result.Inserted++;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row, "General", $"Error: {ex.Message}"));
                    result.Errors++;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.ErrorList.Add(new ImportError(0, "System", $"Critical error: {ex.Message}"));
            result.Errors++;
        }

        return result;
    }

    private bool ValidateSalesHeaders(ExcelWorksheet sheet)
    {
        var headers = new[]
        {
            "InvoiceNumber","SaleDate","ClientId","Subtotal","TaxRate",
            "Total","PaymentMethod","IsPaid","Notes"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            var value = sheet.Cells[1, i + 1].Text.Trim();
            if (!value.Equals(headers[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    // -------------------------------------------------------------------------
    // SALE ITEMS IMPORT
    // -------------------------------------------------------------------------

    public async Task<ImportResult> ImportSaleItemsAsync(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var sheet = package.Workbook.Worksheets[0];
            var rowCount = sheet.Dimension?.Rows ?? 0;

            if (!ValidateSaleItemsHeaders(sheet))
            {
                result.ErrorList.Add(new ImportError(0, "Headers",
                    "Invalid headers. Expected: SalesId, ProductId, Quantity, UnitPrice"));

                result.Errors++;
                return result;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    var salesIdTxt = sheet.Cells[row, 1].Text.Trim();
                    var productIdTxt = sheet.Cells[row, 2].Text.Trim();
                    var qtyTxt = sheet.Cells[row, 3].Text.Trim();
                    var priceTxt = sheet.Cells[row, 4].Text.Trim();

                    if (!int.TryParse(salesIdTxt, out var salesId))
                    {
                        result.ErrorList.Add(new ImportError(row, "SalesId", "Invalid SalesId"));
                        result.Errors++;
                        continue;
                    }

                    if (!int.TryParse(productIdTxt, out var productId))
                    {
                        result.ErrorList.Add(new ImportError(row, "ProductId", "Invalid ProductId"));
                        result.Errors++;
                        continue;
                    }

                    if (!int.TryParse(qtyTxt, out var qty))
                    {
                        result.ErrorList.Add(new ImportError(row, "Quantity", "Invalid Quantity"));
                        result.Errors++;
                        continue;
                    }

                    if (!decimal.TryParse(priceTxt, out var unitPrice))
                    {
                        result.ErrorList.Add(new ImportError(row, "UnitPrice", "Invalid UnitPrice"));
                        result.Errors++;
                        continue;
                    }

                    // Validate FKs
                    if (!await _context.Sales.AnyAsync(s => s.Id == salesId))
                    {
                        result.ErrorList.Add(new ImportError(row, "SalesId", $"Sale with Id {salesId} does not exist"));
                        result.Errors++;
                        continue;
                    }

                    if (!await _context.Products.AnyAsync(p => p.Id == productId))
                    {
                        result.ErrorList.Add(new ImportError(row, "ProductId", $"Product with Id {productId} does not exist"));
                        result.Errors++;
                        continue;
                    }

                    // Upsert
                    var existing = await _context.SaleItems
                        .FirstOrDefaultAsync(si => si.SalesId == salesId && si.ProductId == productId);

                    if (existing != null)
                    {
                        existing.Quantity = qty;
                        existing.UnitPrice = unitPrice;

                        result.Updated++;
                    }
                    else
                    {
                        var newItem = new SaleItems
                        {
                            SalesId = salesId,
                            ProductId = productId,
                            Quantity = qty,
                            UnitPrice = unitPrice
                        };

                        _context.SaleItems.Add(newItem);
                        result.Inserted++;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row, "General", $"Error: {ex.Message}"));
                    result.Errors++;
                }
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.ErrorList.Add(new ImportError(0, "System", $"Critical error: {ex.Message}"));
            result.Errors++;
        }

        return result;
    }

    private bool ValidateSaleItemsHeaders(ExcelWorksheet sheet)
    {
        var headers = new[] { "SalesId", "ProductId", "Quantity", "UnitPrice" };

        for (int i = 0; i < headers.Length; i++)
        {
            var value = sheet.Cells[1, i + 1].Text.Trim();
            if (!value.Equals(headers[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    // -------------------------------------------------------------------------
    // HELPERS
    // -------------------------------------------------------------------------

    private bool IsValidEmail(string email)
    {
        try
        {
            var mail = new System.Net.Mail.MailAddress(email);
            return mail.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
