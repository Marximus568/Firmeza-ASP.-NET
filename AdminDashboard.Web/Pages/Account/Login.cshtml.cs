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

        if (!string.IsNullOrEmpty(result.Token))
        {
            Response.Cookies.Append("access_token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure this is true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }

        _logger.LogInformation("User {Email} logged in successfully with roles: {Roles}", 
            result.Email, string.Join(", ", result.Roles));
        
        // Role-based redirection
        if (result.Roles.Contains(UserRole.Admin))
        {
            _logger.LogInformation("Redirecting admin user {Email} to Admin Dashboard", result.Email);
            return RedirectToPage("/Admin/Index");
        }
        
        if (result.Roles.Contains(UserRole.Client))
        {
            _logger.LogInformation("Redirecting client user {Email} to Products page", result.Email);
            return RedirectToPage("/Admin/Products/Index");
        }
        
        // Default fallback for other roles
        _logger.LogInformation("Redirecting user {Email} to default page", result.Email);
        return LocalRedirect(returnUrl);
    }
}