using AdminDashboard.Application.Product;
using AdminDashboard.Application.Product.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Products;

public class Create : PageModel
{ /// <summary>
    /// Page model for creating a new product.
    /// Only accessible to users with Admin role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IProductServices _productService;

        public CreateModel(IProductServices productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public CreateProductDto Product { get; set; } = new CreateProductDto();

        public void OnGet()
        {
            // Initialize form
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _productService.CreateAsync(Product);
                TempData["SuccessMessage"] = $"Product '{Product.Name}' created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating product: {ex.Message}");
                return Page();
            }
        }
    }
}