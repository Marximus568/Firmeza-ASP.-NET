
using AdminDashboard.Infrastructure.Security;
using AdminDashboardApplication.DTOs.Users;
using AdminDashboardApplication.DTOs.Users.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Admin.Users
{
    /// <summary>
    /// Page model for editing an existing user
    /// </summary>
    public class EditModel : AdminPageModel
    {
        private readonly IUsersService _usersService;

        public EditModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// The user data to update
        /// </summary>
        [BindProperty]
        public UpdateUserDto UpdateUserDto { get; set; } = new UpdateUserDto();

        /// <summary>
        /// Handles GET requests to display the edit form
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _usersService.GetByIdAsync(id.Value);

            if (user == null)
            {
                return NotFound();
            }

            // Map UserDto to UpdateUserDto
            UpdateUserDto = new UpdateUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role
            };

            return Page();
        }

        /// <summary>
        /// Handles POST requests to update the user
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var result = await _usersService.UpdateAsync(UpdateUserDto);

                if (result == null)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = $"User '{result.FullName}' updated successfully!";
                return RedirectToPage("Index");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}