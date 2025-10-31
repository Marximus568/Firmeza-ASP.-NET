using AdminDashboard.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Account;

/// <summary>
/// Logout page model.
/// Handles user sign out.
/// </summary>
[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(IAuthService authService, ILogger<LogoutModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task<IActionResult> OnPost(string? returnUrl = null)
    {
        var userEmail = User.Identity?.Name ?? "Unknown";
        
        await _authService.LogoutAsync();
        
        _logger.LogInformation("User {Email} logged out", userEmail);

        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToPage("/Account/Login");
    }

    public IActionResult OnGet()
    {
        // If accessed via GET, show the logout confirmation page
        return Page();
    }
}