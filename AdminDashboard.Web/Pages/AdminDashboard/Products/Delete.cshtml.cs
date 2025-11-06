using AdminDashboard.Application.Product;
using AdminDashboard.Application.Product.Interfaces;
using AdminDashboard.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.AdminDashboard.Products;

public class Delete : AdminPageModel
{
    public class DeleteModel : PageModel
    {
        private readonly IProductServices _productService;

        public DeleteModel(IProductServices productService)
        {
            _productService = productService;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                var success = await _productService.DeleteAsync(id);

                if (!success)
                {
                    TempData["ErrorMessage"] = "Product not found or could not be deleted.";
                    return RedirectToPage("./Index");
                }

                TempData["SuccessMessage"] = $"Product deleted successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting product: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    } 
}