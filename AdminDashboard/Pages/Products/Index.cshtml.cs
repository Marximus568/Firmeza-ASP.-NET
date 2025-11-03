using AdminDashboard.Application.Product;
using AdminDashboard.Application.Product.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Products;

/// <summary>
/// Page model for listing and searching products.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IProductServices _productService;

    public IndexModel(IProductServices productService)
    {
        _productService = productService;
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
}
