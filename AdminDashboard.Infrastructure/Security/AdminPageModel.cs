using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AdminDashboard.Infrastructure.Security
{
    /// <summary>
    /// Base class for pages restricted to admin users only.
    /// Automatically enforces authentication and role verification.
    /// </summary>
    [Authorize] // Require authentication first
    public abstract class AdminPageModel : PageModel
    {
        public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // If user is not admin, redirect to AccessDenied
            if (userRole != "Admin")
            {
                context.Result = new RedirectToPageResult("/Account/AccessDenied");
                return;
            }

            base.OnPageHandlerExecuting(context);
        }
    }
}