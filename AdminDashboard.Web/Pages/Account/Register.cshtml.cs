using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.UseCases.Auth;
using AdminDashboardApplication.DTOs.Auth.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Account
{
    /// <summary>
    /// Register page model.
    /// Allows new users to create accounts (defaults to Client role).
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly RegisterUserUseCase _registerUseCase;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            RegisterUserUseCase registerUseCase,
            ILogger<RegisterModel> logger)
        {
            _registerUseCase = registerUseCase;
            _logger = logger;
        }

        [BindProperty]
        public RegisterDto Input { get; set; } = new RegisterDto();

        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Handles GET requests for the registration page.
        /// </summary>
        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }

        /// <summary>
        /// Handles POST requests for user registration.
        /// Registers the user and redirects to login on success.
        /// </summary>
        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                // Return the page with validation errors
                return Page();
            }

            // Execute user registration
            var registerResult = await _registerUseCase.ExecuteAsync(Input);

            if (!registerResult.Succeeded)
            {
                // Add any registration errors to the model state
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return Page();
            }

            // Log the successful registration
            _logger.LogInformation("User {Email} created a new account", Input.Email);

            // Store a success message to be displayed on the login page
            TempData["SuccessMessage"] = "Your profile has been created successfully! Please log in.";

            // Redirect to the login page
            return RedirectToPage("/Account/Login", new { returnUrl });
        }
    }
}
