using AdminDashboard.Domain.Entities;
using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Application.Interfaces.ExcelImporter;
using AdminDashboard.Application.DTOs.ExcelImporter;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace AdminDashboard.Infrastructure.ExcelImporter.Services;

/// <summary>
/// Service for importing mixed data from Excel files with automatic entity type detection
/// </summary>
public class MixedDataExcelImporter : IExcelImporter
{
    private readonly AppDbContext _context;

    public MixedDataExcelImporter(AppDbContext context)
    {
        _context = context;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<ImportResult> ImportMixedDataAsync(Stream fileStream)
    {
        var result = new ImportResult();

        try
        {
            using var package = new ExcelPackage(fileStream);
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows ?? 0;
            var colCount = worksheet.Dimension?.Columns ?? 0;

            if (rowCount < 2)
            {
                result.ErrorList.Add(new ImportError(0, "File", "Empty file or no data rows"));
                result.Errors++;
                return result;
            }

            // Phase 1: Read headers
            var headers = new List<string>();
            for (int col = 1; col <= colCount; col++)
            {
                headers.Add(worksheet.Cells[1, col].Text.Trim());
            }

            // Phase 2: Classify all rows
            var classifiedRows = new List<ClassifiedRow>();
            for (int row = 2; row <= rowCount; row++)
            {
                var rowData = new Dictionary<string, string>();
                for (int col = 1; col <= colCount; col++)
                {
                    var header = headers[col - 1];
                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        rowData[header] = worksheet.Cells[row, col].Text.Trim();
                    }
                }

                var classifiedRow = new ClassifiedRow
                {
                    RowNumber = row,
                    Data = rowData,
                    EntityType = DetectEntityType(rowData)
                };

                if (classifiedRow.EntityType == EntityType.Unknown)
                {
                    result.ErrorList.Add(new ImportError(row, "Detection",
                        "Could not determine entity type. Please ensure row has required fields."));
                    result.Errors++;
                    continue;
                }

                classifiedRows.Add(classifiedRow);
            }

            // Phase 3: Process Clients first
            var clientRows = classifiedRows.Where(r => r.EntityType == EntityType.Client).ToList();
            var clientEmailToId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in clientRows)
            {
                try
                {
                    var firstName = row.GetValue("FirstName");
                    var lastName = row.GetValue("LastName");
                    var email = row.GetValue("Email");
                    var phone = row.GetValue("PhoneNumber");
                    var address = row.GetValue("Address");
                    var role = row.GetValue("Role");

                    // Validation
                    if (string.IsNullOrWhiteSpace(firstName))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "FirstName", "FirstName is required"));
                        result.Errors++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(lastName))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "LastName", "LastName is required"));
                        result.Errors++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Email", "Valid email is required"));
                        result.Errors++;
                        continue;
                    }

                    // Upsert
                    var existing = await _context.Users.FirstOrDefaultAsync(c => c.Email.ToLower() == email.ToLower());
                    if (existing != null)
                    {
                        existing.FirstName = firstName;
                        existing.LastName = lastName;
                        existing.PhoneNumber = phone;
                        existing.Address = address;
                        existing.Role = string.IsNullOrWhiteSpace(role) ? "Client" : role;
                        result.Updated++;
                        clientEmailToId[email] = existing.Id;
                    }
                    else
                    {
                        var newClient = new Clients(firstName, lastName, email, phone, address,
                            string.IsNullOrWhiteSpace(role) ? "Client" : role);
                        _context.Users.Add(newClient);
                        await _context.SaveChangesAsync(); // Save to get ID
                        result.Inserted++;
                        clientEmailToId[email] = newClient.Id;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row.RowNumber, "General", ex.Message));
                    result.Errors++;
                }
            }

            // Phase 4: Process Products
            var productRows = classifiedRows.Where(r => r.EntityType == EntityType.Product).ToList();
            var productNameToId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in productRows)
            {
                try
                {
                    var name = row.GetValue("Name");
                    var description = row.GetValue("Description");
                    var priceText = row.GetValue("UnitPrice");
                    var stockText = row.GetValue("Stock");
                    var categoryText = row.GetValue("CategoryId");

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Name", "Name is required"));
                        result.Errors++;
                        continue;
                    }

                    if (!decimal.TryParse(priceText, out var price))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "UnitPrice", "Invalid price format"));
                        result.Errors++;
                        continue;
                    }

                    if (!int.TryParse(stockText, out var stock))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Stock", "Invalid stock format"));
                        result.Errors++;
                        continue;
                    }

                    if (price < 0 || stock < 0)
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Values", "Price and stock cannot be negative"));
                        result.Errors++;
                        continue;
                    }

                    int? categoryId = null;
                    if (!string.IsNullOrWhiteSpace(categoryText) && int.TryParse(categoryText, out var catId))
                        categoryId = catId;

                    // Upsert
                    var existing = await _context.Products.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                    if (existing != null)
                    {
                        existing.Description = description;
                        existing.UnitPrice = price;
                        existing.Stock = stock;
                        existing.CategoryId = categoryId;
                        existing.UpdatedAt = DateTime.UtcNow;
                        result.Updated++;
                        productNameToId[name] = existing.Id;
                    }
                    else
                    {
                        var newProduct = new Products
                        {
                            Name = name,
                            Description = description,
                            UnitPrice = price,
                            Stock = stock,
                            CategoryId = categoryId,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.Products.Add(newProduct);
                        await _context.SaveChangesAsync();
                        result.Inserted++;
                        productNameToId[name] = newProduct.Id;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row.RowNumber, "General", ex.Message));
                    result.Errors++;
                }
            }

            // Phase 5: Process Sales
            var saleRows = classifiedRows.Where(r => r.EntityType == EntityType.Sale).ToList();
            var invoiceToSaleId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in saleRows)
            {
                try
                {
                    var invoice = row.GetValue("InvoiceNumber");
                    var dateText = row.GetValue("SaleDate");
                    var clientIdText = row.GetValue("ClientId");
                    var clientEmail = row.GetValue("ClientEmail"); // Support email reference
                    var subtotalText = row.GetValue("Subtotal");
                    var taxText = row.GetValue("TaxRate");
                    var totalText = row.GetValue("Total");
                    var payment = row.GetValue("PaymentMethod");
                    var isPaidText = row.GetValue("IsPaid");
                    var notes = row.GetValue("Notes");

                    if (string.IsNullOrWhiteSpace(invoice))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "InvoiceNumber", "InvoiceNumber is required"));
                        result.Errors++;
                        continue;
                    }

                    // Resolve ClientId
                    int clientId;
                    if (!string.IsNullOrWhiteSpace(clientIdText) && int.TryParse(clientIdText, out var cId))
                    {
                        clientId = cId;
                    }
                    else if (!string.IsNullOrWhiteSpace(clientEmail) && clientEmailToId.TryGetValue(clientEmail, out var mappedId))
                    {
                        clientId = mappedId;
                    }
                    else
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "ClientId",
                            "ClientId or ClientEmail is required and client must exist"));
                        result.Errors++;
                        continue;
                    }

                    // Validation
                    if (!DateTime.TryParse(dateText, out var saleDate))
                        saleDate = DateTime.UtcNow;

                    if (!decimal.TryParse(subtotalText, out var subtotal))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Subtotal", "Invalid subtotal"));
                        result.Errors++;
                        continue;
                    }

                    decimal taxRate = 0.19m;
                    if (!string.IsNullOrWhiteSpace(taxText))
                        decimal.TryParse(taxText, out taxRate);

                    if (!decimal.TryParse(totalText, out var total))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Total", "Invalid total"));
                        result.Errors++;
                        continue;
                    }

                    bool isPaid = isPaidText.Equals("true", StringComparison.OrdinalIgnoreCase) || isPaidText.Equals("1");

                    // Verify client exists
                    if (!await _context.Users.AnyAsync(c => c.Id == clientId))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "ClientId", $"Client with Id {clientId} not found"));
                        result.Errors++;
                        continue;
                    }

                    // Upsert
                    var existing = await _context.Sales.FirstOrDefaultAsync(s => s.InvoiceNumber == invoice);
                    if (existing != null)
                    {
                        existing.SaleDate = saleDate;
                        existing.ClientId = clientId;
                        existing.Subtotal = subtotal;
                        existing.TaxRate = taxRate;
                        existing.Total = total;
                        existing.PaymentMethod = payment;
                        existing.IsPaid = isPaid;
                        existing.Notes = notes;
                        result.Updated++;
                        invoiceToSaleId[invoice] = existing.Id;
                    }
                    else
                    {
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
                        await _context.SaveChangesAsync();
                        result.Inserted++;
                        invoiceToSaleId[invoice] = newSale.Id;
                    }

                    result.TotalRows++;
                }
                catch (Exception ex)
                {
                    result.ErrorList.Add(new ImportError(row.RowNumber, "General", ex.Message));
                    result.Errors++;
                }
            }

            // Phase 6: Process SaleItems
            var saleItemRows = classifiedRows.Where(r => r.EntityType == EntityType.SaleItem).ToList();

            foreach (var row in saleItemRows)
            {
                try
                {
                    var salesIdText = row.GetValue("SalesId");
                    var invoiceRef = row.GetValue("InvoiceNumber"); // Support invoice reference
                    var productIdText = row.GetValue("ProductId");
                    var productName = row.GetValue("ProductName"); // Support name reference
                    var qtyText = row.GetValue("Quantity");
                    var priceText = row.GetValue("UnitPrice");

                    // Resolve SalesId
                    int salesId;
                    if (!string.IsNullOrWhiteSpace(salesIdText) && int.TryParse(salesIdText, out var sId))
                    {
                        salesId = sId;
                    }
                    else if (!string.IsNullOrWhiteSpace(invoiceRef) && invoiceToSaleId.TryGetValue(invoiceRef, out var mappedSaleId))
                    {
                        salesId = mappedSaleId;
                    }
                    else
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "SalesId",
                            "SalesId or InvoiceNumber required and sale must exist"));
                        result.Errors++;
                        continue;
                    }

                    // Resolve ProductId
                    int productId;
                    if (!string.IsNullOrWhiteSpace(productIdText) && int.TryParse(productIdText, out var pId))
                    {
                        productId = pId;
                    }
                    else if (!string.IsNullOrWhiteSpace(productName) && productNameToId.TryGetValue(productName, out var mappedProductId))
                    {
                        productId = mappedProductId;
                    }
                    else
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "ProductId",
                            "ProductId or ProductName required and product must exist"));
                        result.Errors++;
                        continue;
                    }

                    if (!int.TryParse(qtyText, out var qty))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "Quantity", "Invalid quantity"));
                        result.Errors++;
                        continue;
                    }

                    if (!decimal.TryParse(priceText, out var unitPrice))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "UnitPrice", "Invalid price"));
                        result.Errors++;
                        continue;
                    }

                    // Verify references exist
                    if (!await _context.Sales.AnyAsync(s => s.Id == salesId))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "SalesId", $"Sale {salesId} not found"));
                        result.Errors++;
                        continue;
                    }

                    if (!await _context.Products.AnyAsync(p => p.Id == productId))
                    {
                        result.ErrorList.Add(new ImportError(row.RowNumber, "ProductId", $"Product {productId} not found"));
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
                    result.ErrorList.Add(new ImportError(row.RowNumber, "General", ex.Message));
                    result.Errors++;
                }
            }

            // Final save
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.ErrorList.Add(new ImportError(0, "System", $"Critical error: {ex.Message}"));
            result.Errors++;
        }

        return result;
    }

    /// <summary>
    /// Detects entity type based on fields present in the row
    /// </summary>
    private EntityType DetectEntityType(Dictionary<string, string> rowData)
    {
        bool HasField(params string[] fields) => fields.Any(f =>
            rowData.ContainsKey(f) && !string.IsNullOrWhiteSpace(rowData[f]));

        // Check for Client indicators
        if (HasField("FirstName", "LastName") && HasField("Email") && !HasField("InvoiceNumber", "UnitPrice", "SalesId"))
            return EntityType.Client;

        // Check for Product indicators  
        if (HasField("Name") && HasField("UnitPrice", "Stock") && !HasField("Email", "InvoiceNumber", "SalesId"))
            return EntityType.Product;

        // Check for Sale indicators
        if (HasField("InvoiceNumber") && (HasField("ClientId") || HasField("ClientEmail")) && !HasField("SalesId", "ProductId"))
            return EntityType.Sale;

        // Check for SaleItem indicators
        if (HasField("Quantity") && (HasField("SalesId") ||HasField("InvoiceNumber")) && (HasField("ProductId") || HasField("ProductName")))
            return EntityType.SaleItem;

        return EntityType.Unknown;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    // Placeholder implementations for interface compliance
    public Task<ImportResult> ImportClientsAsync(Stream fileStream) =>
        throw new NotImplementedException("Use ImportMixedDataAsync instead");

    public Task<ImportResult> ImportProductsAsync(Stream fileStream) =>
        throw new NotImplementedException("Use ImportMixedDataAsync instead");

    public Task<ImportResult> ImportSalesAsync(Stream fileStream) =>
        throw new NotImplementedException("Use ImportMixedDataAsync instead");

    public Task<ImportResult> ImportSaleItemsAsync(Stream fileStream) =>
        throw new NotImplementedException("Use ImportMixedDataAsync instead");
}
