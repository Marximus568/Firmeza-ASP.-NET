
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.Users
{
    /// <summary>
    /// Page model for deleting a user
    /// </summary>
    public class DeleteModel : AdminPageModel
    {
        private readonly IUsersService _usersService;

        public DeleteModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// The user to delete
        /// </summary>
        [BindProperty]
        public UserDto? User { get; set; }

        /// <summary>
        /// Handles GET requests to display the delete confirmation page
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await _usersService.GetByIdAsync(id.Value);

            if (User == null)
            {
                return NotFound();
            }

            return Page();
        }

        /// <summary>
        /// Handles POST requests to delete the user
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (User == null || User.Id <= 0)
            {
                return NotFound();
            }

            try
            {
                var result = await _usersService.DeleteAsync(User.Id);

                if (!result)
                {
                    TempData["ErrorMessage"] = "Failed to delete the user.";
                    return RedirectToPage("Index");
                }

                TempData["SuccessMessage"] = $"User '{User.FullName}' deleted successfully!";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while deleting the user: {ex.Message}";
                return RedirectToPage("Index");
            }
        }
    }
}