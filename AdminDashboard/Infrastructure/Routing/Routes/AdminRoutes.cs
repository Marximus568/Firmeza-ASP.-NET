namespace AdminDashboard.Infrastructure.Routing;

public static class AdminRoutes
{
    public static void MapAdminRoutes(this WebApplication app)
    {
        app.MapRazorPages();

        app.MapGet("/admin", () => Results.Redirect("/AdminDashboard/Index"));
    }
}