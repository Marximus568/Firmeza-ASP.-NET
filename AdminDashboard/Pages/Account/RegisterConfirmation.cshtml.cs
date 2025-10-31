using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminDashboard.Pages.Account;

/// <summary>
/// Registration confirmation page.
/// Shows success message after user registration.
/// </summary>
[AllowAnonymous]
public class RegisterConfirmationModel : PageModel
{
    [TempData]
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        // Display confirmation message
    }
}