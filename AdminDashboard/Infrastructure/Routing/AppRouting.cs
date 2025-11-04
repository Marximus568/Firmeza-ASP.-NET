namespace AdminDashboard.Infrastructure.Routing;

public static class AppRouting
{
    /// <summary>
    /// Registers all custom route and authorization conventions for Razor Pages.
    /// </summary>
    public static IServiceCollection AddAppRouting(this IServiceCollection services)
    {
        // Register authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
        });

        // Register Razor Page route conventions
        services.AddRazorPages(options =>
        {
            // Only admin can access Products and Users
            options.Conventions.AuthorizeFolder("/Products", "AdminOnly");
            options.Conventions.AuthorizeFolder("/Shared/Users", "AdminOnly");

            // Public folders (e.g., login, register)
            options.Conventions.AllowAnonymousToFolder("/Account");

            // Default authorization for everything else
            options.Conventions.AuthorizeFolder("/");
        });

        return services;
    }

    /// <summary>
    /// Maps endpoints for Razor Pages and API routes.
    /// </summary>
    public static IEndpointRouteBuilder MapAppRoutes(this IEndpointRouteBuilder app)
    {
        // Map Razor Pages
        app.MapRazorPages();

        // Example health endpoint
        app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

        return app;
    }
}