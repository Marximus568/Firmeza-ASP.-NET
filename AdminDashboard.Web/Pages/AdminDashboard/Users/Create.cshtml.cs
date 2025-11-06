using AdminDashboard.Contracts.Users;
using AdminDashboard.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.AdminDashboard.Users
{
    /// <summary>
    /// Page model for creating a new user
    /// </summary>
    public class CreateModel : AdminPageModel
    {
        private readonly IUsersService _usersService;

        public CreateModel(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// The user data to create
        /// </summary>
        [BindProperty]
        public CreateUserDto CreateUserDto { get; set; } = new CreateUserDto();

        /// <summary>
        /// Handles GET requests to display the create form
        /// </summary>
        public IActionResult OnGet()
        {
            return Page();
        }

        /// <summary>
        /// Handles POST requests to create a new user
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                await _usersService.CreateAsync(CreateUserDto);
                TempData["SuccessMessage"] = $"User '{CreateUserDto.FirstName} {CreateUserDto.LastName}' created successfully!";
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