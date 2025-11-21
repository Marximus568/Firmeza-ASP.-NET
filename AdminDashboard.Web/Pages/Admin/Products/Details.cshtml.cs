using AdminDashboard.Application.Product;
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Products;
using AdminDashboardApplication.DTOs.Products.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.Products;

public class Details : AdminPageModel
{
    public class DetailsModel : PageModel
    {
        private readonly IProductServices _productService;

        public DetailsModel(IProductServices productService)
        {
            _productService = productService;
        }

        public ProductDto Product { get; set; } = new ProductDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToPage("./Index");
            }

            Product = product;
            return Page();
        }
    }
}