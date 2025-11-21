using AdminDashboard.Application.Product;
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using ExcelImporter.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace AdminDashboard.Pages.Admin.Products;

/// <summary>
/// Page model for listing and searching products.
/// </summary>
public class IndexModel : AdminPageModel
{
    private readonly IProductServices _productService;
    private readonly IExcelExporter _excelExporter;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IProductServices productService,
        IExcelExporter excelExporter,
        ILogger<IndexModel> logger)
    {
        _productService = productService;
        _excelExporter = excelExporter;
        _logger = logger;
    }

    public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MinStock { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? MaxStock { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        var filter = new ProductFilterDto
        {
            SearchTerm = SearchTerm,
            MinPrice = MinPrice,
            MaxPrice = MaxPrice,
            MinStock = MinStock,
            MaxStock = MaxStock,
            CategoryId = CategoryId
        };

        Products = await _productService.SearchAsync(filter);
    }

    /// <summary>
    /// Export products to Excel
    /// </summary>
    public async Task<IActionResult> OnGetExportExcelAsync()
    {
        try
        {
            var fileBytes = await _excelExporter.ExportProductsAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Products_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting products to Excel");
            ErrorMessage = "Failed to export products to Excel.";
            return RedirectToPage();
        }
    }
}
