using AdminDashboard.Application.Product;
using AdminDashboard.Application.Product.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Products;

public class Edit : PageModel
{
    /// <summary>
    /// Page model for editing an existing product.
    /// Only accessible to users with Admin role.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IProductServices _productService;

        public EditModel(IProductServices productService)
        {
            _productService = productService;
        }

        [BindProperty] public UpdateProductDto Product { get; set; } = new UpdateProductDto();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToPage("./Index");
            }

            Product = new UpdateProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                UnitPrice = product.UnitPrice,
                Stock = product.Stock,
                CategoryId = product.CategoryId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var success = await _productService.UpdateAsync(Product);

                if (!success)
                {
                    TempData["ErrorMessage"] = "Product not found or could not be updated.";
                    return RedirectToPage("./Index");
                }

                TempData["SuccessMessage"] = $"Product '{Product.Name}' updated successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating product: {ex.Message}");
                return Page();
            }
        }
    }
}