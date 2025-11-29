using System.Security.Claims;
using AdminDashboard.Infrastructure.Persistence.Context;

namespace AdminDashboard.Routing;

/// <summary>
/// Centralizes global routing and authorization for Razor Pages.
/// </summary>
public static class AppRouting
{
    // -------------------------------------------------------
    // [1] Configure authorization and Razor Pages conventions
    // -------------------------------------------------------
    public static IServiceCollection AddAppRouting(this IServiceCollection services)
    {
        // Authorization policies
        services.AddAuthorization(options =>
        {
            // Policy that requires the user to have Role = Admin
            options.AddPolicy("AdminOnly", policy =>
            {
                policy.RequireRole("Admin");
            });
        });

        // Razor Pages conventions
        services.AddRazorPages(options =>
        {
            // Admin-only sections
            options.Conventions.AuthorizeFolder("/Admin/Products", "AdminOnly");
            options.Conventions.AuthorizeFolder("/Admin/Users", "AdminOnly");
            options.Conventions.AuthorizeFolder("/Admin/ExcelImporter", "AdminOnly");
            options.Conventions.AuthorizeFolder("/Admin/Reports", "AdminOnly");
            options.Conventions.AuthorizePage("/Admin/Index", "AdminOnly");

            // Public sections (no authentication required)
            options.Conventions.AllowAnonymousToPage("/Index");      // Home page
            options.Conventions.AllowAnonymousToFolder("/Account");  // Login, register, etc.
        });

        return services;
    }

    // -------------------------------------------------------
    // [2] Map routes and endpoints
    // -------------------------------------------------------
    public static void UseAppRouting(this WebApplication app)
    {
        // Map all Razor Pages
        app.MapRazorPages();

        // Friendly redirects for convenience
        app.MapGet("/admin", static () => Results.Redirect("/Admin/Index"));
        app.MapGet("/products", static () => Results.Redirect("/Admin/Products/Index"));
        app.MapGet("/users", static () => Results.Redirect("/Admin/Users/Index"));

        // Health check endpoint
        app.MapGet("/health", async (AppDbContext db) =>
        {
            var canConnect = await db.Database.CanConnectAsync();
            return Results.Ok(new { status = canConnect ? "Healthy" : "Unreachable" });
        });
    }
}
