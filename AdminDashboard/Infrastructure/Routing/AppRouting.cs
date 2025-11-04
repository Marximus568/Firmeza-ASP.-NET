using AdminDashboard.Infrastructure.Persistence.Context;

namespace AdminDashboard.Infrastructure.Routing;

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
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });

        // Razor Pages conventions
        services.AddRazorPages(options =>
        {
            // 🔒 Admin-only sections
            options.Conventions.AuthorizeFolder("/AdminDashboard/Products", "AdminOnly");
            options.Conventions.AuthorizeFolder("/AdminDashboard/Users", "AdminOnly");

            // 🟢 Public sections (no authentication required)
            options.Conventions.AllowAnonymousToPage("/Index");       // Home page
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

        // Friendly redirects (optional)
        app.MapGet("/admin", static () => Results.Redirect("/AdminDashboard/Index"));
        app.MapGet("/products", static () => Results.Redirect("/AdminDashboard/Products/Index"));
        app.MapGet("/users", static () => Results.Redirect("/AdminDashboard/Users/Index"));

        // Health check endpoint
        app.MapGet("/health", async (AppDbContext db) =>
        {
            var canConnect = await db.Database.CanConnectAsync();
            return Results.Ok(new { status = canConnect ? "Healthy" : "Unreachable" });
        });
    }
}
