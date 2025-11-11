using AdminDashboard.Application.UseCases.Auth;
using AdminDashboard.Domain.ValueObjects;
using AdminDashboardApplication.Auth;
using AdminDashboardApplication.Auth.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Account;

/// <summary>
/// Login page model.
/// Handles user authentication and role-based redirection.
/// </summary>
[AllowAnonymous]
public class LoginModel : PageModel
{
    
    private readonly LoginUserUseCase _loginUseCase;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(LoginUserUseCase loginUseCase, ILogger<LoginModel> logger)
    {
        _loginUseCase = loginUseCase;
        _logger = logger;
    }

    [BindProperty]
    public LoginDto Input { get; set; } = new LoginDto();

    public string? ReturnUrl { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _loginUseCase.ExecuteAsync(Input);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }

        // Check if user is a Client trying to access admin panel
        if (result.Roles.Contains(UserRole.Client) && !result.Roles.Contains(UserRole.Admin))
        {
            _logger.LogWarning(
                "Client user {Email} attempted to access admin panel", 
                result.Email
            );

            // Sign out the user
            await _loginUseCase.ExecuteAsync(new LoginDto 
            { 
                Email = string.Empty, 
                Password = string.Empty 
            });

            ErrorMessage = "You do not have permission to access the admin panel. " +
                          "Please contact an administrator if you believe this is an error.";
            
            return RedirectToPage("/Account/AccessDenied");
        }

        _logger.LogInformation("User {Email} logged in successfully", result.Email);
        
        // Always redirect Admin users to AdminDashboard.Web
        if (result.Roles.Contains(UserRole.Admin))
        {
            // Redirect to the AdminDashboard Index Razor Page
            return RedirectToPage("/AdminDashboard/Index");
        }
        
        
        return LocalRedirect(returnUrl);
    }
}