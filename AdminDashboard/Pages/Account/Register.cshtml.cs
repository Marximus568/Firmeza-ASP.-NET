using AdminDashboard.Application.DTOs.Auth;
using AdminDashboard.Application.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Account;

/// <summary>
/// Register page model.
/// Allows new users to create accounts (defaults to Client role).
/// </summary>
[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly RegisterUserUseCase _registerUseCase;
    private readonly LoginUserUseCase _loginUseCase;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        RegisterUserUseCase registerUseCase,
        LoginUserUseCase loginUseCase,
        ILogger<RegisterModel> logger)
    {
        _registerUseCase = registerUseCase;
        _loginUseCase = loginUseCase;
        _logger = logger;
    }

    [BindProperty]
    public RegisterDto Input { get; set; } = new RegisterDto();

    public string? ReturnUrl { get; set; }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Register the user (defaults to Client role)
        var registerResult = await _registerUseCase.ExecuteAsync(Input);

        if (!registerResult.Succeeded)
        {
            foreach (var error in registerResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return Page();
        }

        _logger.LogInformation("User {Email} created a new account", Input.Email);

        // Automatically log in the user after registration
        var loginDto = new LoginDto
        {
            Email = Input.Email,
            Password = Input.Password,
            RememberMe = false
        };

        var loginResult = await _loginUseCase.ExecuteAsync(loginDto);

        if (loginResult.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in after registration", Input.Email);
            
            // Note: New users are Clients by default and cannot access admin panel
            // They will be redirected to the return URL or shown a message
            TempData["SuccessMessage"] = "Account created successfully! Please contact an administrator for access to the admin panel.";
            return RedirectToPage("/Account/RegisterConfirmation");
        }

        // If auto-login fails, redirect to login page
        return RedirectToPage("/Account/Login", new { returnUrl });
    }
}