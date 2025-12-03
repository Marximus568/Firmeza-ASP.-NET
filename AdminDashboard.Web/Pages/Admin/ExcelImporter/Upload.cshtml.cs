using AdminDashboard.Infrastructure.Persistence.Context;
using AdminDashboard.Application.Interfaces.ExcelImporter;
using AdminDashboard.Application.DTOs.ExcelImporter;
using AdminDashboard.Infrastructure.ExcelImporter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.ExcelImporter;

public class UploadModel : PageModel
{
    private readonly IExcelImporter _importer;
    private readonly IExcelExporter _exporter;
    private readonly AppDbContext _context;
    private readonly ILogger<UploadModel> _logger;

    public UploadModel(
        IExcelImporter importer,
        IExcelExporter exporter,
        AppDbContext context,
        ILogger<UploadModel> logger)
    {
        _importer = importer;
        _exporter = exporter;
        _context = context;
        _logger = logger;
    }

    // Properties for the view
    public ImportResult? Result { get; set; }

    /// <summary>
    /// Display the upload page
    /// </summary>
    public void OnGet()
    {
        // Nothing to do, just show the page
    }

    /// <summary>
    /// Handle file import
    /// </summary>
    [BindProperty]
    public IFormFile? UploadedFile { get; set; }

    [BindProperty]
    public string EntityType { get; set; } = string.Empty;

    public async Task<IActionResult> OnPostAsync()
    {
        if (UploadedFile == null || UploadedFile.Length == 0)
        {
            TempData["Error"] = "Please select a file to import.";
            return RedirectToPage();
        }

        if (!UploadedFile.FileName.EndsWith(".xlsx"))
        {
            TempData["Error"] = "Only .xlsx files are supported.";
            return RedirectToPage();
        }

        try
        {
            using (var stream = UploadedFile.OpenReadStream())
            {
                // For MixedData, use MixedDataExcelImporter directly
                if (EntityType == "MixedData")
                {
                    var mixedImporter = new MixedDataExcelImporter(_context);
                    Result = await mixedImporter.ImportMixedDataAsync(stream);
                }
                else
                {
                    Result = EntityType switch
                    {
                        "Clients" => await _importer.ImportClientsAsync(stream),
                        "Products" => await _importer.ImportProductsAsync(stream),
                        "Sales" => await _importer.ImportSalesAsync(stream),
                        "SaleItems" => await _importer.ImportSaleItemsAsync(stream),
                        _ => throw new ArgumentException("Invalid entity type")
                    };
                }
            }

            // Set success message in TempData instead of modifying the read-only Result.Message
            if (Result.Errors == 0)
            {
                TempData["Success"] = $"Import successful! {Result.TotalRows} rows processed, {Result.Inserted} inserted, {Result.Updated} updated.";
            }
            else
            {
                TempData["Warning"] = $"Import completed with {Result.Errors} errors. {Result.Inserted} inserted, {Result.Updated} updated.";
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing file: {FileName}", UploadedFile.FileName);
            TempData["Error"] = $"Import failed: {ex.Message}";
            return RedirectToPage();
        }
    }

    #region Export Handlers

    /// <summary>
    /// Export all clients to Excel
    /// </summary>
    public async Task<IActionResult> OnGetExportClientsAsync()
    {
        try
        {
            var fileBytes = await _exporter.ExportClientsAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Clients_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting clients");
            TempData["Error"] = "Failed to export clients.";
            return RedirectToPage();
        }
    }

    /// <summary>
    /// Export all products to Excel
    /// </summary>
    public async Task<IActionResult> OnGetExportProductsAsync()
    {
        try
        {
            var fileBytes = await _exporter.ExportProductsAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting products");
            TempData["Error"] = "Failed to export products.";
            return RedirectToPage();
        }
    }

    /// <summary>
    /// Export all sales to Excel
    /// </summary>
    public async Task<IActionResult> OnGetExportSalesAsync()
    {
        try
        {
            var fileBytes = await _exporter.ExportSalesAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Sales_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting sales");
            TempData["Error"] = "Failed to export sales.";
            return RedirectToPage();
        }
    }

    /// <summary>
    /// Export all sale items to Excel
    /// </summary>
    public async Task<IActionResult> OnGetExportSaleItemsAsync()
    {
        try
        {
            var fileBytes = await _exporter.ExportSaleItemsAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SaleItems_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting sale items");
            TempData["Error"] = "Failed to export sale items.";
            return RedirectToPage();
        }
    }

    #endregion
}