
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.AdminDashboard.Users
{
    /// <summary>
    /// Page model for displaying user details
    /// </summary>
    public class DetailsModel : AdminPageModel
    {
        private readonly IUsersService _usersService;

        public DetailsModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// The user to display
        /// </summary>
        public UserDto? User { get; set; }

        /// <summary>
        /// Handles GET requests to display user details
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
    }
}